using Microsoft.Data.Sqlite;

// Mismo esquema que PacienteRepository, para la tabla Medico
public class MedicoRepository(string cadenaConexion)
{
    public List<Medico> Listar()
    {
        var medicos = new List<Medico>();

        using var conexion = new SqliteConnection(cadenaConexion);
        conexion.Open();

        var sql = "SELECT Id, Nombre, Especialidad FROM Medico ORDER BY Nombre";
        using var comando = new SqliteCommand(sql, conexion);
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
