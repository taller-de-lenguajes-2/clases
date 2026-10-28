using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;

public class TurnoCrearViewModel
{
    // int? para que "no eligió nada" llegue como null y lo agarre [Required]
    [Display(Name = "Paciente")]
    [Required(ErrorMessage = "Elegí un paciente.")]
    public int? PacienteId { get; set; }

    [Display(Name = "Médico")]
    [Required(ErrorMessage = "Elegí un médico.")]
    public int? MedicoId { get; set; }

    [Display(Name = "Fecha y hora")]
    [Required(ErrorMessage = "La fecha es obligatoria.")]
    [FechaFutura]                                       // un atributo propio: Validaciones/FechaFuturaAttribute.cs
    public DateTime? Fecha { get; set; } = DateTime.Today.AddDays(1).AddHours(9);

    [StringLength(200, ErrorMessage = "Las observaciones no pueden pasar de {1} caracteres.")]
    public string? Observaciones { get; set; }

    // Las listas no vienen del usuario: no se validan
    [ValidateNever] public List<SelectListItem> Pacientes { get; set; } = [];
    [ValidateNever] public List<SelectListItem> Medicos { get; set; } = [];
}
