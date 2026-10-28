using Microsoft.AspNetCore.Mvc;

// El controlador recibe el pedido, le pide los datos al modelo (vía el repositorio)
// y elige qué vista los muestra. No escribe SQL ni HTML.
public class PacientesController(PacienteRepository repositorio) : Controller
{
    // GET /Pacientes  (o /Pacientes/Index)
    public IActionResult Index()
    {
        List<Paciente> pacientes = repositorio.Listar();
        return View(pacientes);   // Views/Pacientes/Index.cshtml recibe la lista
    }

    // GET /Pacientes/Detalle/3  →  id = 3
    public IActionResult Detalle(int id)
    {
        Paciente? paciente = repositorio.Obtener(id);
        if (paciente is null) return NotFound();

        return View(paciente);    // Views/Pacientes/Detalle.cshtml
    }
}
