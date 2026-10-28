public interface IAutenticacionService
{
    bool Login(string nombreUsuario, string clave);
    void Logout();
    bool EstaAutenticado();
    bool TieneRol(string rol);
}
