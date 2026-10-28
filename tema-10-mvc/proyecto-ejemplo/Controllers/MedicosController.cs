using Microsoft.AspNetCore.Mvc;

public class MedicosController(MedicoRepository repositorio) : Controller
{
    // GET /Medicos
    public IActionResult Index()
    {
        return View(repositorio.Listar());
    }
}
