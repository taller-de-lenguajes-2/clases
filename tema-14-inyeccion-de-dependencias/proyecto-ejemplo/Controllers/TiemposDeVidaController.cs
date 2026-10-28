using Microsoft.AspNetCore.Mvc;

// GET /TiemposDeVida → recargar la página y comparar los códigos
public class TiemposDeVidaController(
    IOperacionTransient transient,
    IOperacionScoped scoped,
    IOperacionSingleton singleton,
    ServicioIntermedio intermedio) : Controller
{
    public IActionResult Index()
    {
        var filas = new List<(string Tiempo, string EnControlador, string EnServicio)>
        {
            ("Transient", transient.Codigo, intermedio.Transient),
            ("Scoped", scoped.Codigo, intermedio.Scoped),
            ("Singleton", singleton.Codigo, intermedio.Singleton)
        };
        return View(filas);
    }
}
