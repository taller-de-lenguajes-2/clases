using Microsoft.AspNetCore.Mvc;

public class PacientesController(PacienteRepository repositorio) : Controller
{
    // GET /Pacientes            → todos
    // GET /Pacientes?texto=ana  → el texto llega por query string
    public IActionResult Index(string? texto)
    {
        List<Paciente> pacientes = string.IsNullOrWhiteSpace(texto)
            ? repositorio.Listar()
            : repositorio.Buscar(texto);

        ViewData["Texto"] = texto;    // para que el buscador muestre lo que se buscó
        return View(pacientes);
    }

    // GET /Pacientes/Detalle/3  → el id llega por la ruta
    public IActionResult Detalle(int id)
    {
        Paciente? paciente = repositorio.Obtener(id);
        if (paciente is null) return NotFound();

        return View(paciente);
    }

    // GET /Pacientes/Crear  → muestra el formulario vacío
    [HttpGet]
    public IActionResult Crear()
    {
        return View();
    }

    // POST /Pacientes/Crear  → los datos llegan en el cuerpo y se arman en un Paciente
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Crear(Paciente paciente)
    {
        // Si falta el nombre, se vuelve a mostrar el formulario (la validación, en el tema 13)
        if (!ModelState.IsValid) return View(paciente);

        int id = repositorio.Insertar(paciente);

        // Después de un POST se redirige: si el usuario recarga, no se inserta dos veces
        return RedirectToAction(nameof(Detalle), new { id });
    }
}
