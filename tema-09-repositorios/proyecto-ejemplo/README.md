# EjemploRepositorios — el patrón repositorio

Una consola .NET 10 sobre la base de la clínica (tema 07) que muestra:

- `IPacienteRepository`: el contrato, qué se puede hacer con los pacientes guardados.
- Dos implementaciones del mismo contrato: `PacienteRepository` (SQLite con ADO.NET, tema 08) y `PacienteRepositoryJson` (un archivo `pacientes.json`).
- `Recepcion`: la lógica del negocio (no da de alta un DNI repetido). Recibe un `IPacienteRepository` y no sabe cuál es.
- `MedicoRepository`: un repositorio por entidad.

## Cómo correrlo

```bash
cd tema-09-repositorios/proyecto-ejemplo
dotnet run
dotnet run -- json
```

El primero guarda los pacientes en `clinica.db` y el segundo en `pacientes.json`. La salida es la misma salvo los Id, porque la base ya trae cinco pacientes y el JSON arranca vacío.
Cada corrida recrea `clinica.db` con el script del tema 07 y borra `pacientes.json`.

## Para mirar en clase

- En `Program.cs`, **una sola línea** elige el repositorio. Todo lo demás no cambia.
- `Recepcion.cs` no tiene ni un `using Microsoft.Data.Sqlite`.
- En `PacienteRepository.cs`, lo que es detalle de ADO.NET (`LeerTodos`, `AgregarParametros`) es `private`.
