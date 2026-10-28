using Microsoft.Data.Sqlite;

// Implementación con SQLite y ADO.NET (tema 08). Todo el SQL de Paciente vive acá.
public class PacienteRepository(string cadenaConexion) : IPacienteRepository
{
    private const string Columnas = "SELECT Id, Nombre, DNI, Telefono FROM Paciente";

    public List<Paciente> Listar()
    {
        using var conexion = new SqliteConnection(cadenaConexion);
        conexion.Open();

        using var comando = new SqliteCommand($"{Columnas} ORDER BY Nombre", conexion);
        return LeerTodos(comando);
    }

    public Paciente? Obtener(int id)
    {
        using var conexion = new SqliteConnection(cadenaConexion);
        conexion.Open();

        using var comando = new SqliteCommand($"{Columnas} WHERE Id = @id", conexion);
        comando.Parameters.AddWithValue("@id", id);
        return LeerTodos(comando).FirstOrDefault();
    }

    public List<Paciente> Buscar(string texto)
    {
        using var conexion = new SqliteConnection(cadenaConexion);
        conexion.Open();

        using var comando = new SqliteCommand($"{Columnas} WHERE Nombre LIKE @texto ORDER BY Nombre", conexion);
        comando.Parameters.AddWithValue("@texto", $"%{texto}%");
        return LeerTodos(comando);
    }

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
        AgregarParametros(comando, paciente);

        return Convert.ToInt32(comando.ExecuteScalar());
    }

    public bool Actualizar(Paciente paciente)
    {
        using var conexion = new SqliteConnection(cadenaConexion);
        conexion.Open();

        var sql = "UPDATE Paciente SET Nombre = @nombre, DNI = @dni, Telefono = @telefono WHERE Id = @id";
        using var comando = new SqliteCommand(sql, conexion);
        AgregarParametros(comando, paciente);
        comando.Parameters.AddWithValue("@id", paciente.Id);

        return comando.ExecuteNonQuery() > 0;
    }

    public bool Eliminar(int id)
    {
        using var conexion = new SqliteConnection(cadenaConexion);
        conexion.Open();

        using var comando = new SqliteCommand("DELETE FROM Paciente WHERE Id = @id", conexion);
        comando.Parameters.AddWithValue("@id", id);

        return comando.ExecuteNonQuery() > 0;
    }

    // Métodos privados: el detalle de ADO.NET no sale de esta clase
    private static void AgregarParametros(SqliteCommand comando, Paciente paciente)
    {
        comando.Parameters.AddWithValue("@nombre", paciente.Nombre);
        comando.Parameters.AddWithValue("@dni", (object?)paciente.DNI ?? DBNull.Value);
        comando.Parameters.AddWithValue("@telefono", (object?)paciente.Telefono ?? DBNull.Value);
    }

    private static List<Paciente> LeerTodos(SqliteCommand comando)
    {
        var pacientes = new List<Paciente>();
        using var lector = comando.ExecuteReader();
        while (lector.Read())
        {
            pacientes.Add(new Paciente
            {
                Id = lector.GetInt32(0),
                Nombre = lector.GetString(1),
                DNI = lector.IsDBNull(2) ? null : lector.GetString(2),
                Telefono = lector.IsDBNull(3) ? null : lector.GetString(3)
            });
        }
        return pacientes;
    }
}
