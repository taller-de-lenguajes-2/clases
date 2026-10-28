using Microsoft.Data.Sqlite;

// El mismo repositorio del tema 08, solo con las lecturas: el controlador no escribe SQL.
public class PacienteRepository(string cadenaConexion)
{
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

    private static Paciente LeerPaciente(SqliteDataReader lector) => new()
    {
        Id = lector.GetInt32(0),
        Nombre = lector.GetString(1),
        DNI = lector.IsDBNull(2) ? null : lector.GetString(2),
        Telefono = lector.IsDBNull(3) ? null : lector.GetString(3)
    };
}
