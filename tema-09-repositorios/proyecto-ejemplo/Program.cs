using Microsoft.Data.Sqlite;

// Tema 09 — Repositorios.
//   dotnet run          → los pacientes en SQLite (clinica.db, recreada en cada corrida)
//   dotnet run -- json  → los pacientes en pacientes.json
const string cadenaConexion = "Data Source=clinica.db";
CrearBase();
File.Delete("pacientes.json");

// La ÚNICA línea que sabe dónde están los datos
IPacienteRepository repositorio = args.Contains("json")
    ? new PacienteRepositoryJson("pacientes.json")
    : new PacienteRepository(cadenaConexion);

Console.WriteLine($"Repositorio: {repositorio.GetType().Name}");

// De acá para abajo, nada cambia según el repositorio elegido
var recepcion = new Recepcion(repositorio);

Titulo("1 · Alta de pacientes");
recepcion.DarDeAlta(new Paciente { Nombre = "Lucía Fernández", DNI = "42000111" });
recepcion.DarDeAlta(new Paciente { Nombre = "Julián Ríos", DNI = "41555666", Telefono = "381-7778899" });
recepcion.DarDeAlta(new Paciente { Nombre = "Lucía Fernández (otra vez)", DNI = "42000111" });

Titulo("2 · Listado");
recepcion.MostrarPacientes();

Titulo("3 · Búsqueda");
recepcion.MostrarBusqueda("ana");
recepcion.MostrarBusqueda("luc");

Titulo("4 · Modificar y borrar");
var lucia = repositorio.Buscar("Lucía").First();
recepcion.CambiarTelefono(lucia.Id, "381-4001122");
recepcion.CambiarTelefono(99, "381-0000000");
recepcion.DarDeBaja(lucia.Id);
recepcion.DarDeBaja(99);

Titulo("5 · Un repositorio por entidad");
var medicos = new MedicoRepository(cadenaConexion);
foreach (var m in medicos.Listar())
    Console.WriteLine($"{m.Nombre,-14} {m.Especialidad}");

static void Titulo(string texto) => Console.WriteLine($"\n── {texto}");

static void CrearBase()
{
    using var conexion = new SqliteConnection(cadenaConexion);
    conexion.Open();
    using var comando = conexion.CreateCommand();
    comando.CommandText = File.ReadAllText(Path.Combine(AppContext.BaseDirectory, "clinica.sql"));
    comando.ExecuteNonQuery();
}
