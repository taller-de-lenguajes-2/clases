using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

public class TurnosController(
    TurnoRepository turnos,
    PacienteRepository pacientes,
    MedicoRepository medicos) : Controller
{
    // GET /Turnos → la vista recibe ViewModels, no entidades
    public IActionResult Index()
    {
        List<TurnoListadoViewModel> filas = turnos.Listar()
            .Select(t => new TurnoListadoViewModel(t))
            .ToList();

        return View(filas);
    }

    // GET /Turnos/Crear → el formulario, con las listas para los desplegables
    [HttpGet]
    public IActionResult Crear()
    {
        var modelo = new TurnoCrearViewModel();
        CargarListas(modelo);
        return View(modelo);
    }

    // POST /Turnos/Crear → llega un TurnoCrearViewModel; de ahí se arma la entidad
    [HttpPost]
    public IActionResult Crear(TurnoCrearViewModel modelo)
    {
        Paciente? paciente = pacientes.Obtener(modelo.PacienteId);
        Medico? medico = medicos.Obtener(modelo.MedicoId);

        // Si algo no cierra, se vuelve a mostrar el formulario (la validación, en el tema 13)
        if (paciente is null || medico is null || !ModelState.IsValid)
        {
            CargarListas(modelo);     // las listas no vuelven en el POST
            return View(modelo);
        }

        var turno = new Turno
        {
            Fecha = modelo.Fecha,
            Observaciones = modelo.Observaciones,
            Paciente = paciente,
            Medico = medico
        };
        turnos.Insertar(turno);

        return RedirectToAction("Detalle", "Pacientes", new { id = paciente.Id });
    }

    private void CargarListas(TurnoCrearViewModel modelo)
    {
        modelo.Pacientes = pacientes.Listar()
            .Select(p => new SelectListItem(p.Nombre, p.Id.ToString()))
            .ToList();
        modelo.Medicos = medicos.Listar()
            .Select(m => new SelectListItem($"{m.Nombre} ({m.Especialidad})", m.Id.ToString()))
            .ToList();
    }
}
