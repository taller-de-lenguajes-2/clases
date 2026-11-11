using Microsoft.AspNetCore.Mvc;

public class PacientesController(PacienteRepository pacientes, TurnoRepository turnos) : Controller
{
    // GET /Pacientes?texto=ana
    public IActionResult Index(string? texto)
    {
        List<Paciente> lista = string.IsNullOrWhiteSpace(texto)
            ? pacientes.Listar()
            : pacientes.Buscar(texto);

        ViewData["Texto"] = texto;
        return View(lista);
    }

    // GET /Pacientes/Detalle/3 → junta dos modelos en un ViewModel
    public IActionResult Detalle(int id)
    {
        Paciente? paciente = pacientes.Obtener(id);
        if (paciente is null) return NotFound();

        var modelo = new PacienteDetalleViewModel(paciente, turnos.ListarPorPaciente(id));
        return View(modelo);
    }

    [HttpGet]
    public IActionResult Crear()
    {
        return View(new PacienteCrearViewModel());
    }

    // El formulario llega en un ViewModel; las reglas de negocio las controla el caso de uso
    [HttpPost]
    public IActionResult Crear(PacienteCrearViewModel modelo)
    {
        // ── [V2] los atributos del ViewModel ya corrieron: si algo falló, ModelState lo tiene ──
        if (!ModelState.IsValid) return View(modelo);

        // ── [V5] el caso de uso avisa si el DNI ya existe (lo garantiza la base: [V6]) ──
        var registrarPaciente = new RegistrarPaciente(pacientes);
        Resultado resultado = registrarPaciente.Ejecutar(modelo.Nombre, modelo.DNI, modelo.Telefono);

        if (!resultado.Ok)
        {
            foreach (ErrorDeRegla error in resultado.Errores)
                ModelState.AddModelError(error.Campo, error.Mensaje);
            return View(modelo);
        }

        return RedirectToAction(nameof(Detalle), new { id = resultado.Id });
    }
}
