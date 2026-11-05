using System.Globalization;
using Microsoft.Data.Sqlite;

// Fechas y números en castellano, en cualquier máquina
CultureInfo.DefaultThreadCurrentCulture = new CultureInfo("es-AR");

var builder = WebApplication.CreateBuilder(args);

// La cadena se lee UNA vez, al arrancar. Si falta en appsettings.json, la app no levanta
// (mejor que un error raro en el primer request)
var cadenaConexion = builder.Configuration.GetConnectionString("Clinica")
    ?? throw new InvalidOperationException("Falta la cadena de conexión 'Clinica' en appsettings.json");
CrearBaseSiNoExiste(cadenaConexion);

// ── El contenedor: qué clase se usa para cada interfaz, y cuánto vive cada objeto ──

// La cadena, envuelta en un tipo propio: un solo objeto para toda la aplicación (es un dato que no cambia)
builder.Services.AddSingleton(new ConexionDb(cadenaConexion));

// Repositorios: uno nuevo por request. Reciben ConexionDb, que también está en el contenedor
builder.Services.AddScoped<IPacienteRepository, PacienteRepository>();
builder.Services.AddScoped<IMedicoRepository, MedicoRepository>();
builder.Services.AddScoped<ITurnoRepository, TurnoRepository>();

// Los usuarios están en una lista en memoria: un solo objeto para toda la aplicación
builder.Services.AddSingleton<IUsuarioRepository, UsuarioRepository>();

// Sesión: los datos quedan en el servidor (en memoria); el navegador guarda solo una cookie con el Id
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(20);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<IAutenticacionService, AutenticacionService>();

// Para la página /TiemposDeVida: la misma clase con los tres tiempos de vida
builder.Services.AddTransient<IOperacionTransient, Operacion>();
builder.Services.AddScoped<IOperacionScoped, Operacion>();
builder.Services.AddSingleton<IOperacionSingleton, Operacion>();
builder.Services.AddScoped<ServicioIntermedio>();      // sin interfaz: se pide por la clase

// MVC: controladores que devuelven vistas (en la Web API era AddControllers)
builder.Services.AddControllersWithViews();

var app = builder.Build();

app.UseStaticFiles();   // sirve lo que está en wwwroot (el CSS)

app.UseSession();       // antes de los controladores: así las acciones pueden usar HttpContext.Session

// Ruteo convencional: /Pacientes/Detalle/3 → PacientesController.Detalle(3)
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();

// Si clinica.db no existe la arma con clinica.sql (el script del tema 07).
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
