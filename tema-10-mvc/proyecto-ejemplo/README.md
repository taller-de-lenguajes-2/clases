# ClinicaMvc — la clínica con ASP.NET Core MVC

La misma base de la clínica (tema 07) y los mismos repositorios con ADO.NET (tema 08), pero ahora el servidor devuelve **páginas HTML** en vez de JSON.
Es solo de lectura: lista y detalle de pacientes, y lista de médicos. Los formularios llegan con los temas 12 y 13.

## Cómo correrlo

```bash
cd tema-10-mvc/proyecto-ejemplo
dotnet run
```

Se abre en `http://localhost:5190`. La primera vez crea `clinica.db` con el script del tema 07 (`tema-07-bases-de-datos/material/clinica.sql`).

## Qué hay en cada parte

| Parte | Archivo | Qué hace |
|---|---|---|
| **Modelo** | `Models/Modelos.cs`, `Repositorios/` | Los datos y el SQL. No sabe nada de HTML |
| **Controlador** | `Controllers/` | Recibe el pedido, le pide los datos al repositorio y elige la vista |
| **Vista** | `Views/<Controlador>/<Acción>.cshtml` | Arma el HTML con Razor a partir del modelo que recibe |
| | `Views/Shared/_Layout.cshtml` | El marco común de todas las páginas (tema 11) |
| | `Program.cs` | `AddControllersWithViews()` y la ruta convencional `{controller=Home}/{action=Index}/{id?}` |

## URLs para probar

| URL | Controlador y acción |
|---|---|
| `/` | `HomeController.Index()` |
| `/Pacientes` | `PacientesController.Index()` |
| `/Pacientes/Detalle/2` | `PacientesController.Detalle(2)` |
| `/Pacientes/Detalle/99` | `404`: el repositorio devuelve `null` |
| `/Medicos` | `MedicosController.Index()` |
