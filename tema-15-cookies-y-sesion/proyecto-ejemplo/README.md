# ClinicaMvc — cookies y sesión

La clínica del tema 13 con inicio de sesión, roles y una cookie propia.

## Cómo correrlo

```bash
cd tema-15-cookies-y-sesion/proyecto-ejemplo
dotnet run
```

Se abre en `http://localhost:5195`. La primera vez crea `clinica.db` con el script del tema 07. **Para volver a los datos originales, borrá `clinica.db`.**

## Usuarios de prueba

| Usuario | Contraseña | Rol | Puede |
|---|---|---|---|
| `recepcion` | `turnos2026` | Recepcion | Ver pacientes y turnos, sacar turnos |
| `admin` | `clinica2026` | Admin | Todo lo anterior y dar de alta pacientes |

Están en `Repositorios/UsuarioRepository.cs`, en memoria y con la contraseña en texto plano **solo para el ejemplo**. En un sistema real van en la base y se guarda un *hash* de la contraseña (por ejemplo con `PasswordHasher<T>` o ASP.NET Core Identity), nunca la contraseña.

## Qué mirar

| Archivo | Qué muestra |
|---|---|
| `Program.cs` | `AddDistributedMemoryCache`, `AddSession` con sus opciones, `AddHttpContextAccessor` y `UseSession` |
| `Servicios/AutenticacionService.cs` | `Login` guarda usuario, nombre y rol en la sesión; `Logout` la limpia |
| `Controllers/LoginController.cs` | El formulario de login, el error genérico, `Url.IsLocalUrl` para el `returnUrl` y `Salir` por POST |
| `Filtros/RequiereLoginAttribute.cs` | Dos filtros: `[RequiereLogin]` (autenticación) y `[RequiereRol("Admin")]` (autorización) |
| `Controllers/PacientesController.cs` · `TurnosController.cs` | Los filtros aplicados al controlador entero o a una acción |
| `Controllers/TurnosController.cs` | Una cookie propia, `UltimoMedico`: `Response.Cookies.Append` y `Request.Cookies` |
| `Views/Shared/_Layout.cshtml` | La vista lee la sesión (`Context.Session`) para mostrar «Hola, Marta · Salir» |

## Para probar

- Sin iniciar sesión, `/Turnos` manda al login; después de ingresar, vuelve a `/Turnos`.
- Con `recepcion`, `/Pacientes/Crear` muestra «Sin permiso». Con `admin`, entra.
- Sacar un turno con un médico y volver a «Sacar un turno»: ese médico aparece elegido. Sigue así después de salir y volver a entrar, porque es una cookie y no la sesión.
- En las herramientas del navegador (Aplicación → Cookies) se ven `.AspNetCore.Session` y `UltimoMedico`.
