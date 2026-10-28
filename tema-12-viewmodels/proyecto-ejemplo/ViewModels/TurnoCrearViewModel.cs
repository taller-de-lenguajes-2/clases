using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

// Escenario 3: la vista necesita MÁS que el modelo.
// El formulario para sacar un turno necesita los campos que se cargan
// y, además, las listas de pacientes y médicos para los desplegables.
public class TurnoCrearViewModel
{
    // Lo que el usuario carga (vuelve en el POST)
    [Display(Name = "Paciente")]
    public int PacienteId { get; set; }

    [Display(Name = "Médico")]
    public int MedicoId { get; set; }

    [Display(Name = "Fecha y hora")]
    public DateTime Fecha { get; set; } = DateTime.Today.AddDays(1).AddHours(9);

    public string? Observaciones { get; set; }

    // Lo que la vista necesita para mostrarse (NO vuelve en el POST: hay que volver a cargarlo)
    public List<SelectListItem> Pacientes { get; set; } = [];
    public List<SelectListItem> Medicos { get; set; } = [];
}
