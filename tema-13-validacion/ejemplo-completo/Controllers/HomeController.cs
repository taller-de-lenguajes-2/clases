using Microsoft.AspNetCore.Mvc;

// GET /  →  {controller=Home}/{action=Index}: es la página de inicio
public class HomeController : Controller
{
    public IActionResult Index()
    {
        return View();   // busca Views/Home/Index.cshtml
    }
}
