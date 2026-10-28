# ClinicaMvc — layout, vistas y formularios

La clínica del tema 10, con lo del tema 11: un buscador por query string, el alta de pacientes con un formulario POST y un layout con pie y sección de scripts.
Todavía no valida los datos: eso es el tema 13.

## Cómo correrlo

```bash
cd tema-11-layout-y-vistas/proyecto-ejemplo
dotnet run
```

Se abre en `http://localhost:5191`. La primera vez crea `clinica.db` con el script del tema 07 (`tema-07-bases-de-datos/material/clinica.sql`).
Los pacientes que cargues quedan guardados. **Para volver a los datos originales, borrá `clinica.db`.**

## Qué mirar

| Archivo | Qué muestra |
|---|---|
| `Controllers/PacientesController.cs` | `Index(string? texto)` por query string, `Detalle(int id)` por la ruta, y `Crear` dos veces: `[HttpGet]` muestra el formulario y `[HttpPost]` recibe un `Paciente`, lo inserta y redirige |
| `Views/Pacientes/Index.cshtml` | Un formulario GET (el buscador), `@if` / `@foreach` y links con `asp-route-id` |
| `Views/Pacientes/Crear.cshtml` | Formulario con `asp-action` y `asp-for`, y `@section Scripts` |
| `Views/Shared/_Layout.cshtml` | `@RenderBody()`, `ViewData["Title"]`, el menú con tag helpers y `RenderSectionAsync("Scripts")` |
| `Views/Home/Index.cshtml` | Un bloque `@{ }` y expresiones implícitas y explícitas |
| `Views/_ViewStart.cshtml` · `Views/_ViewImports.cshtml` | El layout por defecto y los tag helpers para todas las vistas |
| `wwwroot/css/sitio.css` | Un archivo estático, servido por `UseStaticFiles()` |

## URLs para probar

| URL | Qué pasa |
|---|---|
| `/Pacientes?texto=ana` | Busca por nombre |
| `/Pacientes/Detalle/2` | El detalle de un paciente |
| `/Pacientes/Crear` | El formulario; al guardar, redirige al detalle del paciente nuevo |
