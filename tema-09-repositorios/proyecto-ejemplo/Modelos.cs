// Clases modelo: una por tabla de la base clinica.db.
// No saben de dónde vienen sus datos: de eso se encargan los repositorios.

public class Paciente
{
    public int Id { get; set; }
    public string Nombre { get; set; } = "";
    public string? DNI { get; set; }
    public string? Telefono { get; set; }      // puede ser NULL en la base
}

public class Medico
{
    public int Id { get; set; }
    public string Nombre { get; set; } = "";
    public string? Especialidad { get; set; }
}
