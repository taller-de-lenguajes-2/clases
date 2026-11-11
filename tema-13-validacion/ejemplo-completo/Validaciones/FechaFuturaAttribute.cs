using System.ComponentModel.DataAnnotations;

// ── [V3] ATRIBUTO DE VALIDACIÓN PROPIO ──────────────────────────────────
// Se escribe una vez y se usa en cualquier propiedad DateTime: [FechaFutura].
// Corre SOLO en el servidor (no le escribimos la parte de JavaScript):
// el navegador deja pasar una fecha vieja y la frena ModelState.
public class FechaFuturaAttribute : ValidationAttribute
{
    public FechaFuturaAttribute()
        : base("{0}: tiene que ser posterior a este momento.") { }

    public override bool IsValid(object? value)
    {
        // null lo resuelve [Required]: acá solo se mira la fecha si hay una
        return value is not DateTime fecha || fecha > DateTime.Now;
    }
}
