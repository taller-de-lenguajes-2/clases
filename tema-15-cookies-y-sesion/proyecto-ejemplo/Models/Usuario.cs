// Quien usa el sistema (no confundir con Paciente: los pacientes no inician sesión)
public class Usuario
{
    public string NombreUsuario { get; set; } = "";
    public string Nombre { get; set; } = "";
    public string Clave { get; set; } = "";     // ⚠ en texto plano solo para el ejemplo: ver README
    public string Rol { get; set; } = "";       // "Recepcion" o "Admin"
}
