using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("[controller]")]
public class ProductosController(ILogger<ProductosController> logger) : ControllerBase
{
    private static readonly List<Producto> _productos = [new(1, "Yerba", 3500), new(2, "Mate", 12000)];

    [HttpGet("{id}")]
    public IActionResult Get(int id)
    {
        logger.LogDebug("Buscando el producto {ProductoId}", id);

        var producto = _productos.Find(p => p.Id == id)
            ?? throw new KeyNotFoundException($"No existe el producto {id}");

        logger.LogInformation("Se consultó el producto {ProductoId} ({Nombre})", producto.Id, producto.Nombre);
        return Ok(producto);
    }

    [HttpPost]
    public IActionResult Crear(Producto nuevo)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(nuevo.Precio);
        _productos.Add(nuevo);
        logger.LogInformation("Producto creado: {@Producto}", nuevo);
        return Created($"/productos/{nuevo.Id}", nuevo);
    }

    [HttpGet("boom")]
    public IActionResult Boom() => throw new InvalidOperationException("La base de datos no responde");
}

public record Producto(int Id, string Nombre, decimal Precio);
