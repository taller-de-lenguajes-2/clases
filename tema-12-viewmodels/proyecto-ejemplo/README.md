# ClinicaMvc — ViewModels

La clínica del tema 11, ahora con turnos y con un ViewModel para cada pantalla que lo necesita.
Todavía no valida los datos: eso es el tema 13.

## Cómo correrlo

```bash
cd tema-12-viewmodels/proyecto-ejemplo
dotnet run
```

Se abre en `http://localhost:5192`. La primera vez crea `clinica.db` con el script del tema 07. **Para volver a los datos originales, borrá `clinica.db`.**

## Los ViewModels

| ViewModel | Escenario | Dónde se usa |
|---|---|---|
| `TurnoListadoViewModel` | 1 · Simplificar un modelo: de un `Turno` (con su `Paciente` y su `Medico`) a una fila con textos ya formateados | `/Turnos` |
| `PacienteDetalleViewModel` | 2 · Dos modelos en una vista: el `Paciente` y sus `Turno` | `/Pacientes/Detalle/3` |
| `TurnoCrearViewModel` | 3 · La vista necesita más que el modelo: los campos del turno y las listas de pacientes y médicos para los desplegables | `/Turnos/Crear` |
| `PacienteCrearViewModel` | Solo los campos que el usuario puede cargar (sin `Id`): evita el *overposting* | `/Pacientes/Crear` |

## Para mirar en clase

- `Models/` tiene las **entidades** y `ViewModels/` los modelos de cada pantalla. Los repositorios trabajan solo con entidades.
- `TurnosController.Crear` (POST) recibe un `TurnoCrearViewModel`, busca el paciente y el médico con los repositorios y arma el `Turno`.
- Si algo falla, vuelve a mostrar el formulario y **carga de nuevo las listas** (`CargarListas`): el formulario solo manda el valor elegido, no la lista.
- `[Display(Name = "...")]` pone el texto de los `<label asp-for>`.
