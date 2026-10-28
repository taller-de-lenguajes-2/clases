// Los usuarios en memoria, para no tocar la base del tema 07.
// En un sistema real estarían en una tabla, con la clave guardada como hash.
public class UsuarioRepository
{
    private readonly List<Usuario> usuarios =
    [
        new Usuario { NombreUsuario = "recepcion", Nombre = "Marta", Clave = "turnos2026", Rol = "Recepcion" },
        new Usuario { NombreUsuario = "admin", Nombre = "Administración", Clave = "clinica2026", Rol = "Admin" }
    ];

    public Usuario? Obtener(string nombreUsuario) =>
        usuarios.FirstOrDefault(u => u.NombreUsuario == nombreUsuario);
}
