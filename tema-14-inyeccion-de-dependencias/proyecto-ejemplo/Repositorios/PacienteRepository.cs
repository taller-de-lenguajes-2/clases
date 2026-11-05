using Microsoft.Data.Sqlite;

// El repositorio del tema 08: todo el SQL de la tabla Paciente vive acá.
// Recibe la cadena de conexión por el constructor (envuelta en ConexionDb): la pide al contenedor, como cualquier dependencia
public class PacienteRepository(ConexionDb conexionDb) : IPacienteRepository
{
    private readonly string cadenaConexion = conexionDb.Cadena;

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

    // Los % van en el valor del parámetro, no en el SQL
    public List<Paciente> Buscar(string texto)
    {
        var pacientes = new List<Paciente>();

        using var conexion = new SqliteConnection(cadenaConexion);
        conexion.Open();

        var sql = "SELECT Id, Nombre, DNI, Telefono FROM Paciente WHERE Nombre LIKE @texto ORDER BY Nombre";
        using var comando = new SqliteCommand(sql, conexion);
        comando.Parameters.AddWithValue("@texto", $"%{texto}%");
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

    // Para la regla "no hay dos pacientes con el mismo DNI"
    public bool ExisteDni(string dni)
    {
        using var conexion = new SqliteConnection(cadenaConexion);
        conexion.Open();

        using var comando = new SqliteCommand("SELECT COUNT(*) FROM Paciente WHERE DNI = @dni", conexion);
        comando.Parameters.AddWithValue("@dni", dni);
        return Convert.ToInt32(comando.ExecuteScalar()) > 0;
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
        comando.Parameters.AddWithValue("@dni", (object?)paciente.DNI ?? DBNull.Value);
        comando.Parameters.AddWithValue("@telefono", (object?)paciente.Telefono ?? DBNull.Value);

        return Convert.ToInt32(comando.ExecuteScalar());
    }

    private static Paciente LeerPaciente(SqliteDataReader lector) => new()
    {
        Id = lector.GetInt32(0),
        Nombre = lector.GetString(1),
        DNI = lector.IsDBNull(2) ? null : lector.GetString(2),
        Telefono = lector.IsDBNull(3) ? null : lector.GetString(3)
    };
}
