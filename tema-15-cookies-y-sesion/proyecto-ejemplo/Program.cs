using System.Globalization;
using Microsoft.Data.Sqlite;

// Fechas y números en castellano, en cualquier máquina
CultureInfo.DefaultThreadCurrentCulture = new CultureInfo("es-AR");

var builder = WebApplication.CreateBuilder(args);

var cadenaConexion = builder.Configuration.GetConnectionString("Clinica")!;
CrearBaseSiNoExiste(cadenaConexion);

builder.Services.AddScoped(_ => new PacienteRepository(cadenaConexion));
builder.Services.AddScoped(_ => new MedicoRepository(cadenaConexion));
builder.Services.AddScoped(_ => new TurnoRepository(cadenaConexion));
builder.Services.AddSingleton<UsuarioRepository>();     // en memoria: uno solo para toda la app

// Sesión: los datos quedan en el servidor (en memoria); el navegador guarda solo una cookie con el Id
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(20);   // sin actividad por 20 minutos, la sesión se pierde
    options.Cookie.HttpOnly = true;                   // JavaScript no puede leer la cookie de sesión
    options.Cookie.IsEssential = true;                // se manda aunque no se acepten cookies opcionales
});

// Para que el servicio de autenticación llegue a la sesión del request actual
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<AutenticacionService>();

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
