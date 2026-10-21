using System.Reflection;
using Microsoft.Data.Sqlite;

var builder = WebApplication.CreateBuilder(args);

// La cadena de conexión sale de appsettings.json: "Data Source=clinica.db"
var cadenaConexion = builder.Configuration.GetConnectionString("Clinica")!;
CrearBaseSiNoExiste(cadenaConexion);

// Cada repositorio recibe la cadena de conexión y abre sus propias conexiones
builder.Services.AddScoped(_ => new PacienteRepository(cadenaConexion));
builder.Services.AddScoped(_ => new MedicoRepository(cadenaConexion));
builder.Services.AddScoped(_ => new TurnoRepository(cadenaConexion));

builder.Services.AddControllers();
builder.Services.AddSwaggerGen(o => o.IncludeXmlComments(Assembly.GetExecutingAssembly()));

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllers();
app.Run();

// Si clinica.db no existe la arma con clinica.sql (el script del tema 07).
// Para volver a los datos originales: borrar clinica.db y correr de nuevo.
static void CrearBaseSiNoExiste(string cadenaConexion)
{
    var archivo = new SqliteConnectionStringBuilder(cadenaConexion).DataSource;
    if (File.Exists(archivo)) return;

    using var conexion = new SqliteConnection(cadenaConexion);
    conexion.Open();
    using var comando = conexion.CreateCommand();
    comando.CommandText = File.ReadAllText(Path.Combine(AppContext.BaseDirectory, "clinica.sql"));
    comando.ExecuteNonQuery();
}
