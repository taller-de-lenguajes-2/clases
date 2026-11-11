# Clínica — ejemplo completo de validación

La clínica con base de datos (SQLite), dos formularios («Nuevo paciente» y «Sacar un turno») y **cada tipo de validación marcado en el código** con una etiqueta `[V1]` … `[V6]`. Para encontrarlas, buscar `[V` en el proyecto.

A diferencia de `proyecto-ejemplo`, las reglas de negocio no están en el controlador. Siguen el criterio de Robert C. Martin (*Clean Architecture*, cap. 20):

- **Reglas del negocio** (V4): existirían aunque la clínica funcionara con agenda y fichas de papel. Las **decide la entidad**, con sus datos o con los que le pasan.
- **Reglas de la aplicación** (V5): existen porque el proceso está automatizado (qué buscar, en qué orden, qué hacer si falla, cuándo guardar). Las sigue el **caso de uso**, que consigue los datos de la base y **le pregunta** a las entidades.

## Cómo correrlo

```bash
cd tema-13-validacion/ejemplo-completo
dotnet run
```

Se abre en `http://localhost:5196`. La primera vez crea `clinica.db` con el script del tema 07. **Para volver a los datos originales, borrá `clinica.db`.**

## Los seis lugares donde se valida

| | Qué controla | Dónde está | Corre en |
|---|---|---|---|
| **V1** | La forma del dato: obligatorio, largo, formato | Atributos del ViewModel → `data-val-*` → jQuery Validation (`Views/*/Crear.cshtml`, `_ValidationScriptsPartial`) | Navegador |
| **V2** | Lo mismo que V1, otra vez, sin poder saltearlo. Más los errores de tipo del binding (`PacienteId=abc`) | Los mismos atributos, revisados por ASP.NET → `ModelState.IsValid` | Servidor |
| **V3** | Una regla de entrada que no viene con .NET: la fecha tiene que ser futura | `Validaciones/FechaFuturaAttribute.cs` | Solo servidor |
| **V4** | Reglas del negocio, las decide la entidad: «se atiende de lunes a viernes, de 8 a 20» y «un médico no atiende dos turnos a la misma hora» | `Models/Modelos.cs` → `Turno.EnHorarioDeAtencion()` y `Medico.EstaLibre(fecha, agenda)` | Servidor (entidades) |
| **V5** | Reglas de la aplicación, el flujo: buscar el paciente y el médico (¿existen?), traer la agenda del médico y preguntarle, avisar si el DNI ya existe, guardar | `CasosDeUso/SacarTurno.cs` y `CasosDeUso/RegistrarPaciente.cs` | Servidor (casos de uso) |
| **V6** | La última red: `NOT NULL`, `UNIQUE (DNI)`, `FOREIGN KEY` | `clinica.sql` del tema 07 (comentado en los repositorios) | Base de datos |

V1 y V2 salen de **los mismos atributos**: la regla se escribe una sola vez (DRY).

**El DNI único** es una regla del negocio, pero sobre *todos* los pacientes: ninguna entidad sola la puede decidir. La garantiza la base (`UNIQUE`, V6) y el caso de uso pregunta antes de guardar para avisar con un mensaje claro (V5).

## Recorrido de un POST de «Sacar un turno»

```
Navegador ── [V1] jQuery Validation ──► (si pasa, se envía)
   │
   ▼
ASP.NET: binding + atributos ── [V2] [V3] ──► ModelState
   │
   ▼
TurnosController.Crear
   ├─ ModelState inválido → CargarListas + View(modelo)
   └─ SacarTurno.Ejecutar(...)
         ├─ [V5] buscar el paciente y el médico: ¿existen?
         ├─ [V4] turno.EnHorarioDeAtencion()          ← decide la entidad
         ├─ [V5] agenda = turnos.ListarPorMedico(...)  ← el caso de uso trae los datos
         ├─ [V4] medico.EstaLibre(fecha, agenda)      ← decide la entidad
         └─ [V5] turnos.Insertar(turno) ── [V6] FOREIGN KEY / NOT NULL
   ├─ con errores → AddModelError + CargarListas + View(modelo)
   └─ sin errores → RedirectToAction (no hacen falta las listas)
```

## Las listas de los desplegables

El navegador manda solo el valor elegido (`MedicoId=3`), nunca la lista entera. Por eso, en `TurnoCrearViewModel`:

- Las listas tienen `= []` y `[ValidateNever]`. Si se declaran sin inicializar (`public List<SelectListItem> Medicos { get; set; }`), con `<Nullable>enable</Nullable>` ASP.NET las toma como obligatorias y, aunque el usuario cargue todo bien, `ModelState` queda inválido con **«The Medicos field is required.»**. Como ese error no es de ningún campo del formulario, con `asp-validation-summary="ModelOnly"` no se ve: el formulario vuelve sin decir por qué.
- El controlador llama a `CargarListas` **cada vez que devuelve la vista** del formulario: en el GET y en el POST con errores. Cuando sale bien redirige, y ahí no hacen falta.

## Para probar cada una

| | Cómo provocarla | Qué se ve |
|---|---|---|
| V1 | «Nuevo paciente» con el nombre vacío o DNI `12.345` | El navegador avisa sin enviar |
| V2 | Lo mismo con JavaScript desactivado (o borrando los `data-val` con F12) | El servidor devuelve el formulario con el mismo mensaje |
| V2 | Teléfono `abc` | Lo deja pasar el navegador; lo frena el servidor |
| V3 | Turno para una fecha que ya pasó | «Fecha y hora: tiene que ser posterior a este momento.» |
| V4 | Turno un sábado o a las 21:00 | «La clínica atiende de lunes a viernes, de 8 a 20.» |
| V4 | Turno con Laura Sosa el viernes 13/11/2026 a las 15:00 (ya tiene uno): el caso de uso trae la agenda y el médico decide | «Laura Sosa ya tiene un turno a esa hora.» |
| V5 | Turno para un paciente que no existe (con F12, cambiar el `value` de una opción a `99`) | «Ese paciente no existe.» |
| V5 | Paciente con DNI `30111222` (el de Ana Gómez) | «Ya hay un paciente con ese DNI.» |
| V6 | Insertar en DB Browser un paciente con DNI `30111222` | `UNIQUE constraint failed: Paciente.DNI` |

Después de sacar un turno o cargar un paciente, se puede abrir `clinica.db` con DB Browser for SQLite y ver la fila nueva.
