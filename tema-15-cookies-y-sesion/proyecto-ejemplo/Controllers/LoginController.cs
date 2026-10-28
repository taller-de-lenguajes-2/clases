using Microsoft.AspNetCore.Mvc;

public class LoginController(AutenticacionService autenticacion) : Controller
{
    // GET /Login?returnUrl=/Turnos
    [HttpGet]
    public IActionResult Index(string? returnUrl)
    {
        return View(new LoginViewModel { ReturnUrl = returnUrl });
    }

    // POST /Login
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Index(LoginViewModel modelo)
    {
        if (!ModelState.IsValid) return View(modelo);

        if (!autenticacion.Login(modelo.NombreUsuario, modelo.Clave))
        {
            // No decir cuál de los dos está mal: no le damos pistas a quien prueba usuarios
            ModelState.AddModelError("", "Usuario o contraseña incorrectos.");
            return View(modelo);
        }

        // Solo se vuelve a una dirección de NUESTRO sitio (evita redirecciones a otro sitio)
        if (Url.IsLocalUrl(modelo.ReturnUrl)) return Redirect(modelo.ReturnUrl);
        return RedirectToAction("Index", "Home");
    }

    // POST /Login/Salir (es POST: cerrar sesión cambia el estado)
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Salir()
    {
        autenticacion.Logout();
        return RedirectToAction("Index", "Home");
    }
}
