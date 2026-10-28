using System.ComponentModel.DataAnnotations;

// Las reglas de cada campo se escriben UNA vez, acá (DRY).
// ASP.NET las usa en el servidor (ModelState) y las manda al navegador (data-val-*).
public class PacienteCrearViewModel
{
    [Required(ErrorMessage = "El nombre es obligatorio.")]
    [StringLength(100, MinimumLength = 3, ErrorMessage = "El nombre tiene que tener entre {2} y {1} letras.")]
    public string Nombre { get; set; } = "";

    [RegularExpression(@"^\d{7,8}$", ErrorMessage = "El DNI son 7 u 8 números, sin puntos.")]
    public string? DNI { get; set; }

    [Display(Name = "Teléfono")]
    [Phone(ErrorMessage = "El teléfono no tiene un formato válido.")]
    [StringLength(20, ErrorMessage = "El teléfono no puede tener más de {1} caracteres.")]
    public string? Telefono { get; set; }
}
