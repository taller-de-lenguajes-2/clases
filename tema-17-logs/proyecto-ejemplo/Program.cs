using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Serilog reemplaza al logger por defecto y lee su configuración de appsettings.json
builder.Services.AddSerilog(config => config.ReadFrom.Configuration(builder.Configuration));

builder.Services.AddControllers();
builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<ManejadorGlobal>();

var app = builder.Build();

app.UseSerilogRequestLogging();   // una línea de log por cada request
app.UseExceptionHandler();

app.MapControllers();
app.Run();
