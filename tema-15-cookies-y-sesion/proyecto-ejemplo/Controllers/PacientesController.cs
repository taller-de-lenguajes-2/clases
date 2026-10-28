using Microsoft.AspNetCore.Mvc;

[RequiereLogin]          // todas las acciones piden haber iniciado sesión
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

    // Solo administración da de alta pacientes (autorización)
    [HttpGet]
    [RequiereRol("Admin")]
    public IActionResult Crear()
    {
        return View(new PacienteCrearViewModel());
    }

    // El formulario llega en un ViewModel; la entidad se arma acá, con los campos que corresponden
    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequiereRol("Admin")]
    public IActionResult Crear(PacienteCrearViewModel modelo)
    {
        // Una regla que ningún atributo puede saber: hay que consultar la base
        if (!string.IsNullOrEmpty(modelo.DNI) && pacientes.ExisteDni(modelo.DNI))
            ModelState.AddModelError(nameof(modelo.DNI), "Ya hay un paciente con ese DNI.");

        if (!ModelState.IsValid) return View(modelo);

        var paciente = new Paciente
        {
            Nombre = modelo.Nombre,
            DNI = modelo.DNI,
            Telefono = modelo.Telefono
        };
        int id = pacientes.Insertar(paciente);

        return RedirectToAction(nameof(Detalle), new { id });
    }
}
