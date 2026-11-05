using Microsoft.Data.Sqlite;

// Mismo esquema que PacienteRepository, para la tabla Medico
// Recibe la cadena de conexión por el constructor (envuelta en ConexionDb): la pide al contenedor, como cualquier dependencia
public class MedicoRepository(ConexionDb conexionDb) : IMedicoRepository
{
    private readonly string cadenaConexion = conexionDb.Cadena;

    public List<Medico> Listar()
    {
        using var conexion = new SqliteConnection(cadenaConexion);
        conexion.Open();

        using var comando = new SqliteCommand("SELECT Id, Nombre, Especialidad FROM Medico ORDER BY Nombre", conexion);
        return LeerMedicos(comando);
    }

    public Medico? Obtener(int id)
    {
        using var conexion = new SqliteConnection(cadenaConexion);
        conexion.Open();

        using var comando = new SqliteCommand("SELECT Id, Nombre, Especialidad FROM Medico WHERE Id = @id", conexion);
        comando.Parameters.AddWithValue("@id", id);
        return LeerMedicos(comando).FirstOrDefault();
    }

    private static List<Medico> LeerMedicos(SqliteCommand comando)
    {
        var medicos = new List<Medico>();

        using var lector = comando.ExecuteReader();
        while (lector.Read())
        {
            medicos.Add(new Medico
            {
                Id = lector.GetInt32(0),
                Nombre = lector.GetString(1),
                Especialidad = lector.IsDBNull(2) ? null : lector.GetString(2)
            });
        }
        return medicos;
    }
}
