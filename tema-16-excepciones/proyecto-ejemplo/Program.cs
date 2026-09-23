// Tema 16 · Excepciones — ejemplos de la clase (.NET 10)
// Correr con: dotnet run

Titulo("Ejemplo 1 · try-catch con varios catch");
foreach (var entrada in new[] { "42", "hola", "99999999999" })
{
    try
    {
        int numero = int.Parse(entrada);
        Console.WriteLine($"'{entrada}' es un int válido: {numero}");
    }
    catch (FormatException)
    {
        Console.WriteLine($"'{entrada}' no es un número");
    }
    catch (OverflowException)
    {
        Console.WriteLine($"'{entrada}' es demasiado grande para un int");
    }
}

Titulo("Ejemplo 2 · finally se ejecuta siempre");
foreach (var divisor in new[] { 2, 0 })
{
    try
    {
        Console.WriteLine($"10 / {divisor} = {10 / divisor}");
    }
    catch (DivideByZeroException ex)
    {
        Console.WriteLine($"Error: {ex.Message}");
    }
    finally
    {
        Console.WriteLine("finally: esto se ejecuta haya o no excepción");
    }
}

Titulo("Ejemplo 3 · filtro de excepción (when)");
foreach (var codigo in new[] { 404, 500 })
{
    try
    {
        throw new HttpRequestException("Falló la llamada", null, (System.Net.HttpStatusCode)codigo);
    }
    catch (HttpRequestException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
    {
        Console.WriteLine("404: el recurso no existe, seguimos sin él");
    }
    catch (HttpRequestException ex)
    {
        Console.WriteLine($"Otro error HTTP ({(int?)ex.StatusCode}): lo tratamos distinto");
    }
}

Titulo("Ejemplo 4 · throw; conserva el StackTrace, throw ex; lo pisa");
try { RelanzarBien(); }
catch (Exception ex) { Console.WriteLine($"throw;    → origen: {PrimerMetodo(ex)}"); }
try { RelanzarMal(); }
catch (Exception ex) { Console.WriteLine($"throw ex; → origen: {PrimerMetodo(ex)}"); }

Titulo("Ejemplo 5 · envolver con contexto (InnerException)");
try
{
    CargarConfiguracion("config-que-no-existe.json");
}
catch (Exception ex)
{
    Console.WriteLine($"Message:        {ex.Message}");
    Console.WriteLine($"InnerException: {ex.InnerException?.GetType().Name} — {ex.InnerException?.Message}");
}

Titulo("Ejemplo 6 · cláusulas de guarda (ThrowIf…)");
try
{
    var cuenta = new Cuenta("Ana");
    cuenta.Depositar(-50);
}
catch (ArgumentOutOfRangeException ex)
{
    Console.WriteLine($"{ex.GetType().Name}: {ex.Message}");
}
try
{
    var cuenta = new Cuenta("   ");
}
catch (ArgumentException ex)
{
    Console.WriteLine($"{ex.GetType().Name}: {ex.Message}");
}

Titulo("Ejemplo 7 · excepción propia");
try
{
    var cuenta = new Cuenta("Beto");
    cuenta.Depositar(100);
    cuenta.Extraer(250);
}
catch (SaldoInsuficienteException ex)
{
    Console.WriteLine($"{ex.Message} (faltan ${ex.Faltante})");
}

Titulo("Ejemplo 8 · TryParse: sin excepciones cuando el error es esperable");
foreach (var entrada in new[] { "42", "hola" })
{
    if (int.TryParse(entrada, out int numero))
        Console.WriteLine($"'{entrada}' → {numero}");
    else
        Console.WriteLine($"'{entrada}' → no es un número (sin try-catch)");
}

// ---------- auxiliares ----------

static void Titulo(string texto) => Console.WriteLine($"\n=== {texto} ===");

// Nombre del método donde "nació" la excepción según su StackTrace
// (las funciones locales se compilan como <<Main>$>g__Nombre|0_2: nos quedamos con Nombre)
static string PrimerMetodo(Exception ex)
{
    string nombre = new System.Diagnostics.StackTrace(ex).GetFrame(0)?.GetMethod()?.Name ?? "?";
    var m = System.Text.RegularExpressions.Regex.Match(nombre, @"g__(\w+)\|");
    return m.Success ? m.Groups[1].Value : nombre;
}

static void Explota() => throw new InvalidOperationException("Algo salió mal");

static void RelanzarBien()
{
    try { Explota(); }
    catch (Exception) { throw; }
}

static void RelanzarMal()
{
    try { Explota(); }
#pragma warning disable CA2200 // a propósito: mostramos por qué no se hace
    catch (Exception ex) { throw ex; }
#pragma warning restore CA2200
}

static string CargarConfiguracion(string ruta)
{
    try
    {
        return File.ReadAllText(ruta);
    }
    catch (FileNotFoundException ex)
    {
        throw new InvalidOperationException($"No se pudo cargar la configuración desde '{ruta}'", ex);
    }
}

public class Cuenta
{
    public string Titular { get; }
    public decimal Saldo { get; private set; }

    public Cuenta(string titular)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(titular);
        Titular = titular;
    }

    public void Depositar(decimal monto)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(monto);
        Saldo += monto;
    }

    public void Extraer(decimal monto)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(monto);
        if (monto > Saldo)
            throw new SaldoInsuficienteException(monto - Saldo);
        Saldo -= monto;
    }
}

public class SaldoInsuficienteException(decimal faltante)
    : Exception("Saldo insuficiente para la extracción")
{
    public decimal Faltante { get; } = faltante;
}
