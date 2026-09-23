using Microsoft.AspNetCore.Diagnostics;

public class ManejadorGlobal(IProblemDetailsService problemDetails, ILogger<ManejadorGlobal> logger)
    : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext http, Exception ex, CancellationToken ct)
    {
        http.Response.StatusCode = ex switch
        {
            KeyNotFoundException => StatusCodes.Status404NotFound,
            ArgumentException    => StatusCodes.Status400BadRequest,
            _                    => StatusCodes.Status500InternalServerError
        };

        if (http.Response.StatusCode >= 500)
            logger.LogError(ex, "Error no controlado en {Ruta}", http.Request.Path);
        else
            logger.LogWarning("Pedido inválido en {Ruta}: {Mensaje}", http.Request.Path, ex.Message);

        return await problemDetails.TryWriteAsync(new()
        {
            HttpContext = http,
            Exception = ex,
            ProblemDetails = { Title = "Ocurrió un error", Detail = ex.Message }
        });
    }
}
