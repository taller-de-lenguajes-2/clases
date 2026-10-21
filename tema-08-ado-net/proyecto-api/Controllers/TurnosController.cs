using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class TurnosController(TurnoRepository turnos, PacienteRepository pacientes, MedicoRepository medicos) : ControllerBase
{
    /// <summary>Lista los turnos con paciente y médico (JOIN). Con ?fecha=AAAA-MM-DD, solo los de ese día</summary>
    /// <param name="fecha">Opcional: el día a consultar, por ejemplo 2026-10-20</param>
    [HttpGet]
    public IActionResult Listar([FromQuery] DateOnly? fecha) =>
        Ok(fecha is null ? turnos.Listar() : turnos.ListarPorDia(fecha.Value));

    /// <summary>Obtiene un turno por su id, con paciente y médico</summary>
    /// <response code="200">El turno</response>
    /// <response code="404">No existe</response>
    [HttpGet("{id:int}")]
    public IActionResult Obtener(int id)
    {
        var turno = turnos.Obtener(id);
        return turno is null ? NotFound() : Ok(turno);
    }

    /// <summary>Crea un turno</summary>
    /// <response code="201">Creado: el header Location trae su URL</response>
    /// <response code="400">No existe el paciente o el médico</response>
    [HttpPost]
    public IActionResult Crear(Turno turno)
    {
        var error = Validar(turno);
        if (error is not null) return BadRequest(error);

        turno.Id = turnos.Insertar(turno);
        return CreatedAtAction(nameof(Obtener), new { id = turno.Id }, turno);
    }

    /// <summary>Reemplaza los datos de un turno</summary>
    /// <response code="204">Actualizado</response>
    /// <response code="400">No existe el paciente o el médico</response>
    /// <response code="404">No existe el turno</response>
    [HttpPut("{id:int}")]
    public IActionResult Actualizar(int id, Turno turno)
    {
        var error = Validar(turno);
        if (error is not null) return BadRequest(error);

        return turnos.Actualizar(id, turno) ? NoContent() : NotFound();
    }

    /// <summary>Borra un turno</summary>
    /// <response code="204">Borrado</response>
    /// <response code="404">No existe</response>
    [HttpDelete("{id:int}")]
    public IActionResult Eliminar(int id) =>
        turnos.Eliminar(id) ? NoContent() : NotFound();

    // La base también lo rechazaría (FOREIGN KEY), pero así el mensaje dice qué falta
    private string? Validar(Turno turno)
    {
        if (pacientes.Obtener(turno.PacienteId) is null) return $"No existe el paciente {turno.PacienteId}";
        if (medicos.Obtener(turno.MedicoId) is null) return $"No existe el médico {turno.MedicoId}";
        return null;
    }
}
