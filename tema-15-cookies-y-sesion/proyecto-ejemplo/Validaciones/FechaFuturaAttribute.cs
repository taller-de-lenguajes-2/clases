using System.ComponentModel.DataAnnotations;

// Un atributo de validación propio: se escribe una vez y se usa en cualquier propiedad DateTime.
// Este corre solo en el servidor (no tiene versión en JavaScript).
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
