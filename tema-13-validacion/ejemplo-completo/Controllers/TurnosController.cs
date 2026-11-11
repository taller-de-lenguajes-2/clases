using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

// El controlador solo hace de intermediario: recibe el formulario, le pide al caso de uso
// que haga el trabajo y elige qué vista mostrar. Ninguna regla de negocio está acá.
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
        CargarListas(modelo);
        return View(modelo);
    }

    [HttpPost]
    public IActionResult Crear(TurnoCrearViewModel modelo)
    {
        // ── [V2] + [V3] los atributos del ViewModel ya corrieron antes de entrar acá ──
        // (también los errores de tipo: si llega PacienteId=abc, el binding ya dejó un error)
        if (!ModelState.IsValid)
        {
            CargarListas(modelo);     // las listas no vuelven en el POST: se cargan de nuevo
            return View(modelo);
        }

        // Pasó [Required]: acá ya no son null
        int pacienteId = modelo.PacienteId!.Value;
        int medicoId = modelo.MedicoId!.Value;
        DateTime fecha = modelo.Fecha!.Value;

        // ── [V5] el caso de uso sigue el flujo y le pregunta a las entidades [V4] ──
        // (en el tema 14 vamos a ver cómo pedírselo al framework en vez de hacer new)
        var sacarTurno = new SacarTurno(turnos, pacientes, medicos);
        Resultado resultado = sacarTurno.Ejecutar(pacienteId, medicoId, fecha, modelo.Observaciones);

        if (!resultado.Ok)
        {
            // Los errores del caso de uso se muestran igual que los de los atributos
            foreach (ErrorDeRegla error in resultado.Errores)
                ModelState.AddModelError(error.Campo, error.Mensaje);

            CargarListas(modelo);     // de nuevo: si se vuelve a la vista, hacen falta las listas
            return View(modelo);
        }

        // Salió bien: se redirige a otra página, así que acá no hacen falta las listas
        return RedirectToAction("Detalle", "Pacientes", new { id = pacienteId });
    }

    // Se llama CADA vez que se devuelve la vista del formulario (GET y POST con errores)
    private void CargarListas(TurnoCrearViewModel modelo)
    {
        modelo.Pacientes = pacientes.Listar()
            .Select(p => new SelectListItem(p.Nombre, p.Id.ToString()))
            .ToList();
        modelo.Medicos = medicos.Listar()
            .Select(m => new SelectListItem(
                m.Especialidad is null ? m.Nombre : $"{m.Nombre} ({m.Especialidad})",
                m.Id.ToString()))
            .ToList();
    }
}
