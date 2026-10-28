using System.Globalization;
using Microsoft.Data.Sqlite;

// La tabla Turno guarda las claves foráneas; para tener el paciente y el médico hace falta un JOIN
// Recibe la configuración por el constructor: la pide al contenedor, como cualquier dependencia
public class TurnoRepository(IConfiguration configuracion) : ITurnoRepository
{
    private readonly string cadenaConexion = configuracion.GetConnectionString("Clinica")!;

    // La fecha se guarda como texto: 'AAAA-MM-DD HH:MM'
    private const string FormatoFecha = "yyyy-MM-dd HH:mm";

    private const string SelectConJoin = """
        SELECT t.Id, t.Fecha, t.Observaciones,
               p.Id, p.Nombre, p.DNI, p.Telefono,
               m.Id, m.Nombre, m.Especialidad
        FROM Turno t
        INNER JOIN Paciente p ON t.PacienteId = p.Id
        INNER JOIN Medico   m ON t.MedicoId   = m.Id
        """;

    public List<Turno> Listar()
    {
        using var conexion = new SqliteConnection(cadenaConexion);
        conexion.Open();

        using var comando = new SqliteCommand(SelectConJoin + " ORDER BY t.Fecha", conexion);
        return LeerTurnos(comando);
    }

    public List<Turno> ListarPorPaciente(int pacienteId)
    {
        using var conexion = new SqliteConnection(cadenaConexion);
        conexion.Open();

        using var comando = new SqliteCommand(SelectConJoin + " WHERE t.PacienteId = @pacienteId ORDER BY t.Fecha", conexion);
        comando.Parameters.AddWithValue("@pacienteId", pacienteId);
        return LeerTurnos(comando);
    }

    // Para la regla "un médico no atiende dos turnos a la misma hora"
    public bool MedicoOcupado(int medicoId, DateTime fecha)
    {
        using var conexion = new SqliteConnection(cadenaConexion);
        conexion.Open();

        using var comando = new SqliteCommand("SELECT COUNT(*) FROM Turno WHERE MedicoId = @medicoId AND Fecha = @fecha", conexion);
        comando.Parameters.AddWithValue("@medicoId", medicoId);
        comando.Parameters.AddWithValue("@fecha", fecha.ToString(FormatoFecha, CultureInfo.InvariantCulture));
        return Convert.ToInt32(comando.ExecuteScalar()) > 0;
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
        comando.Parameters.AddWithValue("@pacienteId", turno.Paciente.Id);
        comando.Parameters.AddWithValue("@medicoId", turno.Medico.Id);
        comando.Parameters.AddWithValue("@fecha", turno.Fecha.ToString(FormatoFecha, CultureInfo.InvariantCulture));
        comando.Parameters.AddWithValue("@observaciones", (object?)turno.Observaciones ?? DBNull.Value);

        return Convert.ToInt32(comando.ExecuteScalar());
    }

    // Cada fila del JOIN arma un Turno con su Paciente y su Medico adentro
    private static List<Turno> LeerTurnos(SqliteCommand comando)
    {
        var turnos = new List<Turno>();

        using var lector = comando.ExecuteReader();
        while (lector.Read())
        {
            turnos.Add(new Turno
            {
                Id = lector.GetInt32(0),
                Fecha = DateTime.ParseExact(lector.GetString(1), FormatoFecha, CultureInfo.InvariantCulture),
                Observaciones = lector.IsDBNull(2) ? null : lector.GetString(2),
                Paciente = new Paciente
                {
                    Id = lector.GetInt32(3),
                    Nombre = lector.GetString(4),
                    DNI = lector.IsDBNull(5) ? null : lector.GetString(5),
                    Telefono = lector.IsDBNull(6) ? null : lector.GetString(6)
                },
                Medico = new Medico
                {
                    Id = lector.GetInt32(7),
                    Nombre = lector.GetString(8),
                    Especialidad = lector.IsDBNull(9) ? null : lector.GetString(9)
                }
            });
        }
        return turnos;
    }
}
