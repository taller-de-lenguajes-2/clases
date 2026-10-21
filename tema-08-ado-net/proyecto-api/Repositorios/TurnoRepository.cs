using Microsoft.Data.Sqlite;

// La tabla Turno guarda las claves foráneas; para mostrar nombres hace falta un JOIN
public class TurnoRepository(string cadenaConexion)
{
    // La fecha se guarda como texto: 'AAAA-MM-DD HH:MM'
    private const string FormatoFecha = "yyyy-MM-dd HH:mm";

    // El mismo SELECT con JOIN sirve para todos los listados; cambia el WHERE
    private const string SelectDetalle = """
        SELECT t.Id, t.Fecha, t.Observaciones,
               p.Id AS PacienteId, p.Nombre AS PacienteNombre,
               m.Id AS MedicoId,   m.Nombre AS MedicoNombre, m.Especialidad
        FROM Turno t
        INNER JOIN Paciente p ON t.PacienteId = p.Id
        INNER JOIN Medico   m ON t.MedicoId   = m.Id
        """;

    // Todos los turnos, con paciente y médico
    public List<TurnoDetalle> Listar()
    {
        using var conexion = new SqliteConnection(cadenaConexion);
        conexion.Open();

        using var comando = new SqliteCommand(SelectDetalle + " ORDER BY t.Fecha", conexion);
        return LeerDetalles(comando);
    }

    // Los turnos de un día: date() de SQLite se queda con 'AAAA-MM-DD'
    public List<TurnoDetalle> ListarPorDia(DateOnly dia)
    {
        using var conexion = new SqliteConnection(cadenaConexion);
        conexion.Open();

        using var comando = new SqliteCommand(SelectDetalle + " WHERE date(t.Fecha) = @dia ORDER BY t.Fecha", conexion);
        comando.Parameters.AddWithValue("@dia", dia.ToString("yyyy-MM-dd"));
        return LeerDetalles(comando);
    }

    // Los turnos de un paciente, con el nombre del médico
    public List<TurnoDetalle> ListarPorPaciente(int pacienteId)
    {
        using var conexion = new SqliteConnection(cadenaConexion);
        conexion.Open();

        using var comando = new SqliteCommand(SelectDetalle + " WHERE t.PacienteId = @pacienteId ORDER BY t.Fecha", conexion);
        comando.Parameters.AddWithValue("@pacienteId", pacienteId);
        return LeerDetalles(comando);
    }

    // Un turno por Id, con paciente y médico
    public TurnoDetalle? Obtener(int id)
    {
        using var conexion = new SqliteConnection(cadenaConexion);
        conexion.Open();

        using var comando = new SqliteCommand(SelectDetalle + " WHERE t.Id = @id", conexion);
        comando.Parameters.AddWithValue("@id", id);
        return LeerDetalles(comando).FirstOrDefault();
    }

    public int Insertar(Turno turno)
    {
        using var conexion = new SqliteConnection(cadenaConexion);
        conexion.Open();

        var sql = """
            INSERT INTO Turno (PacienteId, MedicoId, Fecha, Observaciones)
            VALUES (@pacienteId, @medicoId, @fecha, @observaciones)
            RETURNING Id
            """;
        using var comando = new SqliteCommand(sql, conexion);
        CargarParametros(comando, turno);

        return Convert.ToInt32(comando.ExecuteScalar());
    }

    public bool Actualizar(int id, Turno turno)
    {
        using var conexion = new SqliteConnection(cadenaConexion);
        conexion.Open();

        var sql = """
            UPDATE Turno
            SET PacienteId = @pacienteId, MedicoId = @medicoId,
                Fecha = @fecha, Observaciones = @observaciones
            WHERE Id = @id
            """;
        using var comando = new SqliteCommand(sql, conexion);
        CargarParametros(comando, turno);
        comando.Parameters.AddWithValue("@id", id);

        return comando.ExecuteNonQuery() > 0;
    }

    public bool Eliminar(int id)
    {
        using var conexion = new SqliteConnection(cadenaConexion);
        conexion.Open();

        var sql = "DELETE FROM Turno WHERE Id = @id";
        using var comando = new SqliteCommand(sql, conexion);
        comando.Parameters.AddWithValue("@id", id);

        return comando.ExecuteNonQuery() > 0;
    }

    // INSERT y UPDATE usan los mismos parámetros
    private static void CargarParametros(SqliteCommand comando, Turno turno)
    {
        comando.Parameters.AddWithValue("@pacienteId", turno.PacienteId);
        comando.Parameters.AddWithValue("@medicoId", turno.MedicoId);
        comando.Parameters.AddWithValue("@fecha", turno.Fecha.ToString(FormatoFecha));
        comando.Parameters.AddWithValue("@observaciones", (object?)turno.Observaciones ?? DBNull.Value);
    }

    // Cada fila del JOIN se convierte en un TurnoDetalle
    private static List<TurnoDetalle> LeerDetalles(SqliteCommand comando)
    {
        var turnos = new List<TurnoDetalle>();

        using var lector = comando.ExecuteReader();
        while (lector.Read())
        {
            turnos.Add(new TurnoDetalle
            {
                Id = Convert.ToInt32(lector["Id"]),
                Fecha = DateTime.Parse(lector["Fecha"].ToString()!),
                Observaciones = lector["Observaciones"] as string,
                PacienteId = Convert.ToInt32(lector["PacienteId"]),
                Paciente = lector["PacienteNombre"].ToString()!,
                MedicoId = Convert.ToInt32(lector["MedicoId"]),
                Medico = lector["MedicoNombre"].ToString()!,
                Especialidad = lector["Especialidad"] as string
            });
        }
        return turnos;
    }
}
