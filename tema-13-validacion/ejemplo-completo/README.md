# Clínica — ejemplo completo de validación

La clínica con base de datos (SQLite), dos formularios («Nuevo paciente» y «Sacar un turno») y **cada tipo de validación marcado en el código** con una etiqueta `[V1]` … `[V6]`. Para encontrarlas, buscar `[V` en el proyecto.

A diferencia de `proyecto-ejemplo`, las reglas de negocio no están en el controlador: están en la **entidad** (las que solo necesitan sus propios datos) y en los **casos de uso** (las que necesitan consultar la base).

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
| **V4** | Regla de negocio que solo necesita los datos de la entidad: «se atiende de lunes a viernes, de 8 a 20» | `Models/Modelos.cs` → `Turno.EnHorarioDeAtencion()` | Servidor (dominio) |
| **V5** | Reglas de negocio que necesitan otros datos: el paciente y el médico existen, el médico no está ocupado, el DNI no está repetido | `CasosDeUso/SacarTurno.cs` y `CasosDeUso/RegistrarPaciente.cs` | Servidor (caso de uso) |
| **V6** | La última red: `NOT NULL`, `UNIQUE (DNI)`, `FOREIGN KEY` | `clinica.sql` del tema 07 (comentado en los repositorios) | Base de datos |

V1 y V2 salen de **los mismos atributos**: la regla se escribe una sola vez (DRY).

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
         ├─ [V5] ¿existen el paciente y el médico?
         ├─ [V4] turno.EnHorarioDeAtencion()
         ├─ [V5] ¿el médico ya tiene turno a esa hora?
         └─ turnos.Insertar(turno) ── [V6] FOREIGN KEY / NOT NULL
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
| V5 | Turno con Laura Sosa el viernes 13/11/2026 a las 15:00 (ya tiene uno) | «Laura Sosa ya tiene un turno a esa hora.» |
| V5 | Paciente con DNI `30111222` (el de Ana Gómez) | «Ya hay un paciente con ese DNI.» |
| V6 | Insertar en DB Browser un paciente con DNI `30111222` | `UNIQUE constraint failed: Paciente.DNI` |

Después de sacar un turno o cargar un paciente, se puede abrir `clinica.db` con DB Browser for SQLite y ver la fila nueva.
