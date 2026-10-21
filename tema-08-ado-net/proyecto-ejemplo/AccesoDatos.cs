using Microsoft.Data.Sqlite;

// Todos los ejemplos del tema 08: cada método abre su conexión,
// ejecuta un comando y la cierra (using) al terminar.
public class AccesoDatos(string cadenaConexion)
{
    // SELECT de varias filas → List<Paciente>
    public List<Paciente> ListarPacientes()
    {
        var pacientes = new List<Paciente>();

        using var conexion = new SqliteConnection(cadenaConexion);
        conexion.Open();

        var sql = "SELECT Id, Nombre, DNI, Telefono FROM Paciente ORDER BY Nombre";
        using var comando = new SqliteCommand(sql, conexion);
        using var lector = comando.ExecuteReader();

        while (lector.Read())
        {
            pacientes.Add(LeerPaciente(lector));
        }
        return pacientes;
    }

    // SELECT de una fila → un objeto o null
    public Paciente? ObtenerPaciente(int id)
    {
        using var conexion = new SqliteConnection(cadenaConexion);
        conexion.Open();

        var sql = "SELECT Id, Nombre, DNI, Telefono FROM Paciente WHERE Id = @id";
        using var comando = new SqliteCommand(sql, conexion);
        comando.Parameters.AddWithValue("@id", id);

        using var lector = comando.ExecuteReader();
        return lector.Read() ? LeerPaciente(lector) : null;
    }

    // LIKE con parámetro: los % van en el valor, no en el SQL
    public List<Paciente> BuscarPacientes(string texto)
    {
        var pacientes = new List<Paciente>();

        using var conexion = new SqliteConnection(cadenaConexion);
        conexion.Open();

        var sql = "SELECT Id, Nombre, DNI, Telefono FROM Paciente WHERE Nombre LIKE @texto";
        using var comando = new SqliteCommand(sql, conexion);
        comando.Parameters.AddWithValue("@texto", $"%{texto}%");

        using var lector = comando.ExecuteReader();
        while (lector.Read())
        {
            pacientes.Add(LeerPaciente(lector));
        }
        return pacientes;
    }

    // ExecuteScalar: un único valor
    public int ContarPacientes()
    {
        using var conexion = new SqliteConnection(cadenaConexion);
        conexion.Open();

        using var comando = new SqliteCommand("SELECT COUNT(*) FROM Paciente", conexion);
        return Convert.ToInt32(comando.ExecuteScalar());
    }

    // INSERT … RETURNING Id: inserta y devuelve el Id generado
    public int InsertarPaciente(Paciente paciente)
    {
        using var conexion = new SqliteConnection(cadenaConexion);
        conexion.Open();

        var sql = """
            INSERT INTO Paciente (Nombre, DNI, Telefono)
            VALUES (@nombre, @dni, @telefono)
            RETURNING Id
            """;
        using var comando = new SqliteCommand(sql, conexion);
        comando.Parameters.AddWithValue("@nombre", paciente.Nombre);
        comando.Parameters.AddWithValue("@dni", (object?)paciente.DNI ?? DBNull.Value);
        comando.Parameters.AddWithValue("@telefono", (object?)paciente.Telefono ?? DBNull.Value);

        return Convert.ToInt32(comando.ExecuteScalar());
    }

    // UPDATE: ExecuteNonQuery devuelve cuántas filas cambió
    public bool ActualizarTelefono(int id, string telefono)
    {
        using var conexion = new SqliteConnection(cadenaConexion);
        conexion.Open();

        var sql = "UPDATE Paciente SET Telefono = @telefono WHERE Id = @id";
        using var comando = new SqliteCommand(sql, conexion);
        comando.Parameters.AddWithValue("@telefono", telefono);
        comando.Parameters.AddWithValue("@id", id);

        return comando.ExecuteNonQuery() > 0;
    }

    // DELETE
    public bool EliminarPaciente(int id)
    {
        using var conexion = new SqliteConnection(cadenaConexion);
        conexion.Open();

        var sql = "DELETE FROM Paciente WHERE Id = @id";
        using var comando = new SqliteCommand(sql, conexion);
        comando.Parameters.AddWithValue("@id", id);

        return comando.ExecuteNonQuery() > 0;
    }

    // JOIN: cada fila arma un Turno con su Paciente y su Medico
    public List<Turno> ListarTurnos()
    {
        var turnos = new List<Turno>();

        using var conexion = new SqliteConnection(cadenaConexion);
        conexion.Open();

        var sql = """
            SELECT t.Id, t.Fecha, t.Observaciones,
                   p.Id AS PacienteId, p.Nombre AS PacienteNombre,
                   m.Id AS MedicoId,   m.Nombre AS MedicoNombre, m.Especialidad
            FROM Turno t
            INNER JOIN Paciente p ON t.PacienteId = p.Id
            INNER JOIN Medico   m ON t.MedicoId   = m.Id
            ORDER BY t.Fecha
            """;
        using var comando = new SqliteCommand(sql, conexion);
        using var lector = comando.ExecuteReader();

        while (lector.Read())
        {
            turnos.Add(new Turno
            {
                Id = Convert.ToInt32(lector["Id"]),
                Fecha = DateTime.Parse(lector["Fecha"].ToString()!),
                Observaciones = lector["Observaciones"] as string,
                Paciente = new Paciente
                {
                    Id = Convert.ToInt32(lector["PacienteId"]),
                    Nombre = lector["PacienteNombre"].ToString()!
                },
                Medico = new Medico
                {
                    Id = Convert.ToInt32(lector["MedicoId"]),
                    Nombre = lector["MedicoNombre"].ToString()!,
                    Especialidad = lector["Especialidad"] as string
                }
            });
        }
        return turnos;
    }

    // ✗ NO HACER: SQL armado concatenando lo que escribió el usuario
    public List<Paciente> BuscarPorDniInseguro(string dni)
    {
        var pacientes = new List<Paciente>();

        using var conexion = new SqliteConnection(cadenaConexion);
        conexion.Open();

        var sql = "SELECT Id, Nombre, DNI, Telefono FROM Paciente WHERE DNI = '" + dni + "'";
        using var comando = new SqliteCommand(sql, conexion);
        using var lector = comando.ExecuteReader();
        while (lector.Read())
        {
            pacientes.Add(LeerPaciente(lector));
        }
        return pacientes;
    }

    // ✓ Lo mismo con parámetro
    public List<Paciente> BuscarPorDni(string dni)
    {
        var pacientes = new List<Paciente>();

        using var conexion = new SqliteConnection(cadenaConexion);
        conexion.Open();

        var sql = "SELECT Id, Nombre, DNI, Telefono FROM Paciente WHERE DNI = @dni";
        using var comando = new SqliteCommand(sql, conexion);
        comando.Parameters.AddWithValue("@dni", dni);
        using var lector = comando.ExecuteReader();
        while (lector.Read())
        {
            pacientes.Add(LeerPaciente(lector));
        }
        return pacientes;
    }

    // Pasa una fila del lector a un objeto (las columnas NULL llegan como DBNull)
    private static Paciente LeerPaciente(SqliteDataReader lector) => new()
    {
        Id = Convert.ToInt32(lector["Id"]),
        Nombre = lector["Nombre"].ToString()!,
        DNI = lector["DNI"] as string,
        Telefono = lector["Telefono"] as string
    };
}
