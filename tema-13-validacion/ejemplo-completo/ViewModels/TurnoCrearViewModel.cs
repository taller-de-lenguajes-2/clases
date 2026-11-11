using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;

// El formulario «Sacar un turno».
// ── [V1] + [V2] VALIDACIÓN DE ENTRADA ───────────────────────────────────
// Los atributos se escriben UNA vez (DRY) y corren en dos lados:
//   [V1] en el navegador: asp-for los convierte en data-val-* y jQuery Validation avisa sin enviar
//   [V2] en el servidor: antes de entrar a la acción, ASP.NET los revisa y deja el resultado en ModelState
// Solo controlan la FORMA del dato (que esté, que no sea muy largo…), no las reglas de negocio.
public class TurnoCrearViewModel
{
    // int? para que "no eligió nada" llegue como null y lo agarre [Required]
    [Display(Name = "Paciente")]
    [Required(ErrorMessage = "Elegí un paciente.")]                        // [V1] [V2]
    public int? PacienteId { get; set; }

    [Display(Name = "Médico")]
    [Required(ErrorMessage = "Elegí un médico.")]                          // [V1] [V2]
    public int? MedicoId { get; set; }

    [Display(Name = "Fecha y hora")]
    [Required(ErrorMessage = "La fecha es obligatoria.")]                  // [V1] [V2]
    [FechaFutura]                                                          // [V3] atributo propio, solo servidor
    public DateTime? Fecha { get; set; } = DateTime.Today.AddDays(1).AddHours(9);

    [StringLength(200, ErrorMessage = "Las observaciones no pueden pasar de {1} caracteres.")]  // [V1] [V2]
    public string? Observaciones { get; set; }

    // ── LAS LISTAS DE LOS DESPLEGABLES ───────────────────────────────────
    // El navegador manda solo el valor elegido (MedicoId=3), NUNCA la lista entera.
    // Entonces, en el POST:
    //   1. Estas listas llegan sin nada. Si fueran "List<SelectListItem> Medicos { get; set; }"
    //      (sin "= []"), llegarían en null y, con <Nullable>enable</Nullable>, ASP.NET las toma
    //      como obligatorias: ModelState queda inválido con «The Medicos field is required.»
    //      aunque el usuario haya cargado todo bien.
    //   2. [ValidateNever] le dice a ASP.NET que no las valide: no las carga el usuario.
    //   3. Si hay que volver a mostrar el formulario, el controlador las vuelve a llenar
    //      (CargarListas). No hace falta mandarlas ida y vuelta.
    [ValidateNever] public List<SelectListItem> Pacientes { get; set; } = [];
    [ValidateNever] public List<SelectListItem> Medicos { get; set; } = [];
}
