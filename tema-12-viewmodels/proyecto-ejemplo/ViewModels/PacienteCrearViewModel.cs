using System.ComponentModel.DataAnnotations;

// Solo los campos que el usuario puede cargar: no tiene Id.
// Si alguien agrega un campo "Id" al formulario a mano, no hay dónde ponerlo.
public class PacienteCrearViewModel
{
    public string Nombre { get; set; } = "";
    public string? DNI { get; set; }

    [Display(Name = "Teléfono")]
    public string? Telefono { get; set; }
}
