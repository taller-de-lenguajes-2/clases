using Microsoft.Data.Sqlite;

// Un repositorio por entidad: el SQL de Medico vive acá, no en PacienteRepository
public class MedicoRepository(string cadenaConexion)
{
    public List<Medico> Listar()
    {
        var medicos = new List<Medico>();

        using var conexion = new SqliteConnection(cadenaConexion);
        conexion.Open();

        using var comando = new SqliteCommand("SELECT Id, Nombre, Especialidad FROM Medico ORDER BY Nombre", conexion);
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
