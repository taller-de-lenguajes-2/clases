using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.Sqlite;

[ApiController]
[Route("api/[controller]")]
public class MedicosController(MedicoRepository medicos) : ControllerBase
{
    private const int SqliteConstraint = 19;

    /// <summary>Lista todos los médicos</summary>
    [HttpGet]
    public IActionResult Listar() => Ok(medicos.Listar());

    /// <summary>Obtiene un médico por su id</summary>
    /// <response code="200">El médico</response>
    /// <response code="404">No existe</response>
    [HttpGet("{id:int}")]
    public IActionResult Obtener(int id)
    {
        var medico = medicos.Obtener(id);
        return medico is null ? NotFound() : Ok(medico);
    }

    /// <summary>Crea un médico</summary>
    /// <response code="201">Creado: el header Location trae su URL</response>
    /// <response code="400">Falta el nombre</response>
    [HttpPost]
    public IActionResult Crear(Medico medico)
    {
        medico.Id = medicos.Insertar(medico);
        return CreatedAtAction(nameof(Obtener), new { id = medico.Id }, medico);
    }

    /// <summary>Reemplaza los datos de un médico</summary>
    /// <response code="204">Actualizado</response>
    /// <response code="404">No existe</response>
    [HttpPut("{id:int}")]
    public IActionResult Actualizar(int id, Medico medico) =>
        medicos.Actualizar(id, medico) ? NoContent() : NotFound();

    /// <summary>Borra un médico</summary>
    /// <response code="204">Borrado</response>
    /// <response code="404">No existe</response>
    /// <response code="409">Tiene turnos: la clave foránea no deja borrarlo</response>
    [HttpDelete("{id:int}")]
    public IActionResult Eliminar(int id)
    {
        try
        {
            return medicos.Eliminar(id) ? NoContent() : NotFound();
        }
        catch (SqliteException ex) when (ex.SqliteErrorCode == SqliteConstraint)
        {
            return Conflict($"El médico {id} tiene turnos: borralos primero");
        }
    }
}
