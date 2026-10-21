using Microsoft.Data.Sqlite;

// Tema 08 — ADO.NET con SQLite. Cada corrida recrea clinica.db
// a partir de clinica.sql (el script del tema 07).
const string cadenaConexion = "Data Source=clinica.db";
CrearBase();

var datos = new AccesoDatos(cadenaConexion);

Titulo("1 · SELECT → List<Paciente>");
foreach (var p in datos.ListarPacientes())
    Console.WriteLine($"{p.Id}  {p.Nombre,-14} tel: {p.Telefono ?? "(sin teléfono)"}");

Titulo("2 · SELECT por Id → un objeto o null");
Console.WriteLine(datos.ObtenerPaciente(3)?.Nombre ?? "no existe");
Console.WriteLine(datos.ObtenerPaciente(99)?.Nombre ?? "no existe");

Titulo("3 · LIKE con parámetro: \"ana\"");
foreach (var p in datos.BuscarPacientes("ana"))
    Console.WriteLine(p.Nombre);

Titulo("4 · ExecuteScalar: COUNT(*)");
Console.WriteLine($"Pacientes: {datos.ContarPacientes()}");

Titulo("5 · INSERT … RETURNING Id");
var nuevoId = datos.InsertarPaciente(new Paciente { Nombre = "Lucía Fernández", DNI = "42000111" });
Console.WriteLine($"Nuevo paciente con Id {nuevoId}");

Titulo("6 · UPDATE y DELETE: filas afectadas");
Console.WriteLine($"Actualizar Id {nuevoId}: {datos.ActualizarTelefono(nuevoId, "381-4001122")}");
Console.WriteLine($"Actualizar Id 99: {datos.ActualizarTelefono(99, "381-0000000")}");
Console.WriteLine($"Eliminar Id {nuevoId}: {datos.EliminarPaciente(nuevoId)}");

Titulo("7 · JOIN → Turno con Paciente y Medico");
foreach (var t in datos.ListarTurnos())
    Console.WriteLine($"{t.Fecha:dd/MM HH:mm}  {t.Paciente.Nombre,-12} → {t.Medico.Nombre} ({t.Medico.Especialidad})");

Titulo("8 · Integridad referencial: borrar un paciente con turnos");
try
{
    datos.EliminarPaciente(1);
}
catch (SqliteException ex)
{
    Console.WriteLine($"SqliteException: {ex.Message}");
}

Titulo("9 · Inyección SQL");
var entrada = "' OR '1'='1";
Console.WriteLine($"Concatenando:  {datos.BuscarPorDniInseguro(entrada).Count} pacientes  ← ¡todos!");
Console.WriteLine($"Con parámetro: {datos.BuscarPorDni(entrada).Count} pacientes");

static void Titulo(string texto) => Console.WriteLine($"\n── {texto}");

static void CrearBase()
{
    using var conexion = new SqliteConnection(cadenaConexion);
    conexion.Open();
    using var comando = conexion.CreateCommand();
    comando.CommandText = File.ReadAllText(Path.Combine(AppContext.BaseDirectory, "clinica.sql"));
    comando.ExecuteNonQuery();
}
