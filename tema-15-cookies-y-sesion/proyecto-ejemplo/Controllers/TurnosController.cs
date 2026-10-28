using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

[RequiereLogin]
public class TurnosController(
    TurnoRepository turnos,
    PacienteRepository pacientes,
    MedicoRepository medicos) : Controller
{
    public IActionResult Index()
    {
        List<TurnoListadoViewModel> filas = turnos.Listar()
            .Select(t => new TurnoListadoViewModel(t))
            .ToList();

        return View(filas);
    }

    [HttpGet]
    public IActionResult Crear()
    {
        var modelo = new TurnoCrearViewModel();

        // Una cookie propia: el último médico elegido, para dejarlo preseleccionado
        if (int.TryParse(Request.Cookies[CookieUltimoMedico], out int medicoId))
            modelo.MedicoId = medicoId;

        CargarListas(modelo);
        return View(modelo);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Crear(TurnoCrearViewModel modelo)
    {
        // 1 · Los atributos del ViewModel ya corrieron: si algo falló, ModelState lo tiene
        if (!ModelState.IsValid)
        {
            CargarListas(modelo);
            return View(modelo);
        }

        // Pasó [Required]: acá ya no son null
        int pacienteId = modelo.PacienteId!.Value;
        int medicoId = modelo.MedicoId!.Value;
        DateTime fecha = modelo.Fecha!.Value;

        // 2 · Reglas que necesitan datos de la base: se agregan a mano
        Paciente? paciente = pacientes.Obtener(pacienteId);
        Medico? medico = medicos.Obtener(medicoId);

        if (paciente is null)
            ModelState.AddModelError(nameof(modelo.PacienteId), "Ese paciente no existe.");
        if (medico is null)
            ModelState.AddModelError(nameof(modelo.MedicoId), "Ese médico no existe.");
        else if (turnos.MedicoOcupado(medicoId, fecha))
            ModelState.AddModelError(nameof(modelo.Fecha), $"{medico.Nombre} ya tiene un turno a esa hora.");

        if (!ModelState.IsValid)
        {
            CargarListas(modelo);
            return View(modelo);
        }

        var turno = new Turno
        {
            Fecha = fecha,
            Observaciones = modelo.Observaciones,
            Paciente = paciente!,
            Medico = medico!
        };
        turnos.Insertar(turno);

        // La cookie la guarda el navegador por 30 días, aunque se cierre la sesión
        Response.Cookies.Append(CookieUltimoMedico, medicoId.ToString(), new CookieOptions
        {
            Expires = DateTimeOffset.Now.AddDays(30),
            HttpOnly = true,
            SameSite = SameSiteMode.Lax
        });

        return RedirectToAction("Detalle", "Pacientes", new { id = pacienteId });
    }

    private const string CookieUltimoMedico = "UltimoMedico";

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
