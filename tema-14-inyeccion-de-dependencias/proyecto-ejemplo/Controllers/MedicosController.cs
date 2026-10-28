using Microsoft.AspNetCore.Mvc;

public class MedicosController(IMedicoRepository repositorio) : Controller
{
    // GET /Medicos
    public IActionResult Index()
    {
        return View(repositorio.Listar());
    }
}
