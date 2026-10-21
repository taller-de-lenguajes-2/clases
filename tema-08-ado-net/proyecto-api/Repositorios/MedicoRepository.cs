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
            medicos.Add(LeerMedico(lector));
        }
        return medicos;
    }

    public Medico? Obtener(int id)
    {
        using var conexion = new SqliteConnection(cadenaConexion);
        conexion.Open();

        var sql = "SELECT Id, Nombre, Especialidad FROM Medico WHERE Id = @id";
        using var comando = new SqliteCommand(sql, conexion);
        comando.Parameters.AddWithValue("@id", id);

        using var lector = comando.ExecuteReader();
        return lector.Read() ? LeerMedico(lector) : null;
    }

    public int Insertar(Medico medico)
    {
        using var conexion = new SqliteConnection(cadenaConexion);
        conexion.Open();

        var sql = "INSERT INTO Medico (Nombre, Especialidad) VALUES (@nombre, @especialidad) RETURNING Id";
        using var comando = new SqliteCommand(sql, conexion);
        comando.Parameters.AddWithValue("@nombre", medico.Nombre);
        comando.Parameters.AddWithValue("@especialidad", (object?)medico.Especialidad ?? DBNull.Value);

        return Convert.ToInt32(comando.ExecuteScalar());
    }

    public bool Actualizar(int id, Medico medico)
    {
        using var conexion = new SqliteConnection(cadenaConexion);
        conexion.Open();

        var sql = "UPDATE Medico SET Nombre = @nombre, Especialidad = @especialidad WHERE Id = @id";
        using var comando = new SqliteCommand(sql, conexion);
        comando.Parameters.AddWithValue("@nombre", medico.Nombre);
        comando.Parameters.AddWithValue("@especialidad", (object?)medico.Especialidad ?? DBNull.Value);
        comando.Parameters.AddWithValue("@id", id);

        return comando.ExecuteNonQuery() > 0;
    }

    public bool Eliminar(int id)
    {
        using var conexion = new SqliteConnection(cadenaConexion);
        conexion.Open();

        var sql = "DELETE FROM Medico WHERE Id = @id";
        using var comando = new SqliteCommand(sql, conexion);
        comando.Parameters.AddWithValue("@id", id);

        return comando.ExecuteNonQuery() > 0;
    }

    private static Medico LeerMedico(SqliteDataReader lector) => new()
    {
        Id = Convert.ToInt32(lector["Id"]),
        Nombre = lector["Nombre"].ToString()!,
        Especialidad = lector["Especialidad"] as string
    };
}
