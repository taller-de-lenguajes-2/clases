# ClinicaMvc — inyección de dependencias

La clínica del tema 15, con todo armado por el contenedor de dependencias de .NET: los controladores y los servicios dependen de **interfaces**, y `Program.cs` dice qué clase usar para cada una y cuánto vive cada objeto.

## Cómo correrlo

```bash
cd tema-14-inyeccion-de-dependencias/proyecto-ejemplo
dotnet run
```

Se abre en `http://localhost:5194`. La primera vez crea `clinica.db` con el script del tema 07. Los usuarios de prueba son los del tema 15: `recepcion` / `turnos2026` y `admin` / `clinica2026` (en memoria y en texto plano, solo para el ejemplo).

## Qué mirar

| Archivo | Qué muestra |
|---|---|
| `Program.cs` | El registro: `AddScoped<IPacienteRepository, PacienteRepository>()`, `AddSingleton<IUsuarioRepository, UsuarioRepository>()`, etc. |
| `Repositorios/Interfaces.cs` · `Servicios/IAutenticacionService.cs` | Las abstracciones de las que dependen los controladores |
| `Repositorios/PacienteRepository.cs` | Recibe `IConfiguration` por el constructor: el contenedor resuelve también las dependencias de las dependencias |
| `Controllers/TurnosController.cs` | Pide tres repositorios por el constructor, todos por su interfaz |
| `Servicios/TiemposDeVida.cs` · `Controllers/TiemposDeVidaController.cs` | La misma clase registrada como Transient, Scoped y Singleton |

## Para probar

Abrir `/TiemposDeVida` (está en el menú) y recargar varias veces:

- **Transient**: un código distinto en cada lugar y en cada recarga.
- **Scoped**: el mismo código en el controlador y en `ServicioIntermedio` (mismo request), otro en la recarga siguiente.
- **Singleton**: siempre el mismo, hasta que se reinicia la aplicación.
