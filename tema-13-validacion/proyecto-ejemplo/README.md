# ClinicaMvc — validación

La clínica del tema 12 con los formularios validados en el servidor y en el navegador.

## Cómo correrlo

```bash
cd tema-13-validacion/proyecto-ejemplo
dotnet run
```

Se abre en `http://localhost:5193`. La primera vez crea `clinica.db` con el script del tema 07. **Para volver a los datos originales, borrá `clinica.db`.**

## Qué mirar

| Archivo | Qué muestra |
|---|---|
| `ViewModels/PacienteCrearViewModel.cs` | `[Required]`, `[StringLength]`, `[RegularExpression]`, `[Phone]` y mensajes con `{0}`, `{1}`, `{2}` |
| `ViewModels/TurnoCrearViewModel.cs` | `int?` + `[Required]` para los desplegables, `[ValidateNever]` en las listas y `[FechaFutura]` |
| `Validaciones/FechaFuturaAttribute.cs` | Un atributo de validación propio |
| `Controllers/PacientesController.cs` | `ModelState.IsValid` y una regla que consulta la base: DNI repetido |
| `Controllers/TurnosController.cs` | `AddModelError` para paciente o médico inexistente y médico ocupado a esa hora |
| `Views/Pacientes/Crear.cshtml` · `Views/Turnos/Crear.cshtml` | `asp-validation-for`, `asp-validation-summary` y el partial de scripts |
| `Views/Shared/_ValidationScriptsPartial.cshtml` | jQuery, jQuery Validation y Unobtrusive desde un CDN |

## Para probar

- Dejar el nombre vacío o poner `12.345` en el DNI: el navegador avisa sin enviar.
- Teléfono `abc` o un turno para ayer: el navegador lo deja pasar y lo frena el servidor.
- DNI `30111222` (Ana Gómez): «Ya hay un paciente con ese DNI».
- Turno con Laura Sosa el 13/11/2026 a las 15:00 (un horario futuro en el que ya tiene turno): «Laura Sosa ya tiene un turno a esa hora».
- Con JavaScript desactivado en el navegador, todo sigue validándose en el servidor.
