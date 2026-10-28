// Las entidades del dominio: una por tabla de la base clinica.db.
// Representan la clínica, no una pantalla: ninguna sabe de HTML ni de formularios.

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

public class Turno
{
    public int Id { get; set; }
    public DateTime Fecha { get; set; }
    public string? Observaciones { get; set; }

    // Composición (tema 02): el turno tiene un paciente y un médico
    public Paciente Paciente { get; set; } = new();
    public Medico Medico { get; set; } = new();
}
