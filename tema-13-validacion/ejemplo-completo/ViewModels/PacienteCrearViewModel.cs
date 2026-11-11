using System.ComponentModel.DataAnnotations;

// El formulario «Nuevo paciente».
// ── [V1] + [V2] VALIDACIÓN DE ENTRADA ───────────────────────────────────
// Las reglas de cada campo se escriben UNA vez, acá (DRY).
// ASP.NET las usa en el servidor (ModelState) [V2] y las manda al navegador (data-val-*) [V1].
public class PacienteCrearViewModel
{
    [Required(ErrorMessage = "El nombre es obligatorio.")]                                         // [V1] [V2]
    [StringLength(100, MinimumLength = 3, ErrorMessage = "El nombre tiene que tener entre {2} y {1} caracteres.")]  // [V1] [V2]
    public string Nombre { get; set; } = "";

    [RegularExpression(@"^\d{7,8}$", ErrorMessage = "El DNI son 7 u 8 números, sin puntos.")]      // [V1] [V2]
    public string? DNI { get; set; }
    // Que el DNI no esté repetido NO va acá: necesita la base → caso de uso [V5]

    [Display(Name = "Teléfono")]
    [Phone(ErrorMessage = "El teléfono no tiene un formato válido.")]                              // [V2] (en el navegador no se controla)
    [StringLength(20, ErrorMessage = "El teléfono no puede tener más de {1} caracteres.")]         // [V1] [V2]
    public string? Telefono { get; set; }
}
