// Para VER los tres tiempos de vida: cada objeto recibe un código al azar cuando se crea.
// Si dos lugares muestran el mismo código, recibieron el MISMO objeto.

public interface IOperacionTransient { string Codigo { get; } }
public interface IOperacionScoped { string Codigo { get; } }
public interface IOperacionSingleton { string Codigo { get; } }

// Una sola clase implementa las tres interfaces; cada una se registra con otro tiempo de vida
public class Operacion : IOperacionTransient, IOperacionScoped, IOperacionSingleton
{
    public string Codigo { get; } = Guid.NewGuid().ToString()[..6];
}

// Otro servicio que pide las mismas tres: así se ve qué pasa dentro de un mismo request
public class ServicioIntermedio(
    IOperacionTransient transient,
    IOperacionScoped scoped,
    IOperacionSingleton singleton)
{
    public string Transient => transient.Codigo;
    public string Scoped => scoped.Codigo;
    public string Singleton => singleton.Codigo;
}
