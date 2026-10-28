using System.ComponentModel.DataAnnotations;

public class LoginViewModel
{
    [Display(Name = "Usuario")]
    [Required(ErrorMessage = "Ingresá tu usuario.")]
    public string NombreUsuario { get; set; } = "";

    [Display(Name = "Contraseña")]
    [Required(ErrorMessage = "Ingresá tu contraseña.")]
    [DataType(DataType.Password)]                       // asp-for genera type="password"
    public string Clave { get; set; } = "";

    // A dónde volver después de iniciar sesión
    public string? ReturnUrl { get; set; }
}
