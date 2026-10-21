using System.ComponentModel.DataAnnotations;

// Clases modelo: una por tabla de la base clinica.db

public class Paciente
{
    public int Id { get; set; }
    [Required] public string Nombre { get; set; } = "";
    public string? DNI { get; set; }
    public string? Telefono { get; set; }      // puede ser NULL en la base
}

public class Medico
{
    public int Id { get; set; }
    [Required] public string Nombre { get; set; } = "";
    public string? Especialidad { get; set; }
}

public class Turno
{
    public int Id { get; set; }
    public int PacienteId { get; set; }        // claves foráneas: así se guarda en la tabla
    public int MedicoId { get; set; }
    public DateTime Fecha { get; set; }        // en la base: TEXT 'AAAA-MM-DD HH:MM'
    public string? Observaciones { get; set; }
}

// No es una tabla: es lo que devuelve un JOIN de Turno con Paciente y Medico
public class TurnoDetalle
{
    public int Id { get; set; }
    public DateTime Fecha { get; set; }
    public string? Observaciones { get; set; }
    public int PacienteId { get; set; }
    public string Paciente { get; set; } = "";
    public int MedicoId { get; set; }
    public string Medico { get; set; } = "";
    public string? Especialidad { get; set; }
}
