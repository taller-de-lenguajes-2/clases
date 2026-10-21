using Microsoft.Data.Sqlite;

// Todo el SQL de la tabla Paciente vive acá: el controlador no escribe SQL.
// Cada método abre su conexión, ejecuta un comando y la cierra (using) al terminar.
public class PacienteRepository(string cadenaConexion)
{
    // SELECT de varias filas → List<Paciente>
    public List<Paciente> Listar()
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
    public Paciente? Obtener(int id)
    {
        using var conexion = new SqliteConnection(cadenaConexion);
        conexion.Open();

        var sql = "SELECT Id, Nombre, DNI, Telefono FROM Paciente WHERE Id = @id";
        using var comando = new SqliteCommand(sql, conexion);
        comando.Parameters.AddWithValue("@id", id);

        using var lector = comando.ExecuteReader();
        return lector.Read() ? LeerPaciente(lector) : null;
    }

    // INSERT … RETURNING Id: ExecuteScalar devuelve el Id que generó la base
    public int Insertar(Paciente paciente)
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
        comando.Parameters.AddWithValue("@dni", (object?)paciente.DNI ?? DBNull.Value);   // null de C# → NULL de SQL
        comando.Parameters.AddWithValue("@telefono", (object?)paciente.Telefono ?? DBNull.Value);

        return Convert.ToInt32(comando.ExecuteScalar());
    }

    // UPDATE: ExecuteNonQuery devuelve cuántas filas cambió (0 = no existía)
    public bool Actualizar(int id, Paciente paciente)
    {
        using var conexion = new SqliteConnection(cadenaConexion);
        conexion.Open();

        var sql = "UPDATE Paciente SET Nombre = @nombre, DNI = @dni, Telefono = @telefono WHERE Id = @id";
        using var comando = new SqliteCommand(sql, conexion);
        comando.Parameters.AddWithValue("@nombre", paciente.Nombre);
        comando.Parameters.AddWithValue("@dni", (object?)paciente.DNI ?? DBNull.Value);
        comando.Parameters.AddWithValue("@telefono", (object?)paciente.Telefono ?? DBNull.Value);
        comando.Parameters.AddWithValue("@id", id);

        return comando.ExecuteNonQuery() > 0;
    }

    // DELETE: si el paciente tiene turnos, la clave foránea lo impide (SqliteException)
    public bool Eliminar(int id)
    {
        using var conexion = new SqliteConnection(cadenaConexion);
        conexion.Open();

        var sql = "DELETE FROM Paciente WHERE Id = @id";
        using var comando = new SqliteCommand(sql, conexion);
        comando.Parameters.AddWithValue("@id", id);

        return comando.ExecuteNonQuery() > 0;
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
