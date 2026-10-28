using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

// Un filtro: corre ANTES de la acción. Si no hay sesión iniciada, no deja pasar
// y manda al login, recordando a dónde quería ir.
// Se pone sobre un controlador (todas sus acciones) o sobre una acción.
public class RequiereLoginAttribute : ActionFilterAttribute
{
    public override void OnActionExecuting(ActionExecutingContext context)
    {
        var sesion = context.HttpContext.Session;
        if (sesion.GetString(AutenticacionService.ClaveUsuario) is null)
        {
            var pedido = context.HttpContext.Request;
            context.Result = new RedirectToActionResult("Index", "Login",
                new { returnUrl = pedido.Path + pedido.QueryString });
        }
    }
}

// Autorización: además de haber iniciado sesión, hace falta un rol
public class RequiereRolAttribute(string rol) : ActionFilterAttribute
{
    public override void OnActionExecuting(ActionExecutingContext context)
    {
        if (context.HttpContext.Session.GetString(AutenticacionService.ClaveRol) != rol)
        {
            context.Result = new ViewResult { ViewName = "SinPermiso" };
        }
    }
}
