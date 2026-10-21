using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.Sqlite;

// El controlador atiende HTTP y le pide los datos al repositorio: no escribe SQL
[ApiController]
[Route("api/[controller]")]
public class PacientesController(PacienteRepository pacientes, TurnoRepository turnos) : ControllerBase
{
    // Código de error de SQLite cuando se viola una restricción (UNIQUE, FOREIGN KEY…)
    private const int SqliteConstraint = 19;

    /// <summary>Lista todos los pacientes</summary>
    [HttpGet]
    public IActionResult Listar() => Ok(pacientes.Listar());

    /// <summary>Obtiene un paciente por su id</summary>
    /// <response code="200">El paciente</response>
    /// <response code="404">No existe</response>
    [HttpGet("{id:int}")]
    public IActionResult Obtener(int id)
    {
        var paciente = pacientes.Obtener(id);
        return paciente is null ? NotFound() : Ok(paciente);
    }

    /// <summary>Turnos de un paciente, con el nombre del médico (JOIN)</summary>
    /// <response code="200">Los turnos (puede ser una lista vacía)</response>
    /// <response code="404">No existe el paciente</response>
    [HttpGet("{id:int}/turnos")]
    public IActionResult Turnos(int id)
    {
        if (pacientes.Obtener(id) is null) return NotFound();
        return Ok(turnos.ListarPorPaciente(id));
    }

    /// <summary>Crea un paciente</summary>
    /// <response code="201">Creado: el header Location trae su URL</response>
    /// <response code="400">Faltan datos (por ejemplo, el nombre)</response>
    /// <response code="409">Ya hay un paciente con ese DNI</response>
    [HttpPost]
    public IActionResult Crear(Paciente paciente)
    {
        try
        {
            paciente.Id = pacientes.Insertar(paciente);
            return CreatedAtAction(nameof(Obtener), new { id = paciente.Id }, paciente);
        }
        catch (SqliteException ex) when (ex.SqliteErrorCode == SqliteConstraint)
        {
            return Conflict($"Ya existe un paciente con DNI {paciente.DNI}");
        }
    }

    /// <summary>Reemplaza los datos de un paciente</summary>
    /// <response code="204">Actualizado</response>
    /// <response code="404">No existe</response>
    /// <response code="409">Ya hay otro paciente con ese DNI</response>
    [HttpPut("{id:int}")]
    public IActionResult Actualizar(int id, Paciente paciente)
    {
        try
        {
            return pacientes.Actualizar(id, paciente) ? NoContent() : NotFound();
        }
        catch (SqliteException ex) when (ex.SqliteErrorCode == SqliteConstraint)
        {
            return Conflict($"Ya existe otro paciente con DNI {paciente.DNI}");
        }
    }

    /// <summary>Borra un paciente</summary>
    /// <response code="204">Borrado</response>
    /// <response code="404">No existe</response>
    /// <response code="409">Tiene turnos: la clave foránea no deja borrarlo</response>
    [HttpDelete("{id:int}")]
    public IActionResult Eliminar(int id)
    {
        try
        {
            return pacientes.Eliminar(id) ? NoContent() : NotFound();
        }
        catch (SqliteException ex) when (ex.SqliteErrorCode == SqliteConstraint)
        {
            return Conflict($"El paciente {id} tiene turnos: borralos primero");
        }
    }
}
