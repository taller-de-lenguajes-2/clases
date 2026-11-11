// Las entidades del dominio: una por tabla de la base clinica.db.
// Representan la clínica, no una pantalla: ninguna sabe de HTML ni de formularios.
//
// ── [V4] REGLAS DEL NEGOCIO ─────────────────────────────────────────────
// Las que existirían aunque la clínica funcionara con agenda y fichas de papel.
// Las DECIDE la entidad, con sus propios datos o con los que le pasan.
// La entidad no consulta la base: si necesita más datos, se los trae el caso de uso.

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

    // [V4] «Un médico no atiende dos turnos a la misma hora».
    // Con la agenda de papel también valía. El médico decide mirando su agenda;
    // la agenda (los turnos que ya tiene) se la pasa el caso de uso.
    public bool EstaLibre(DateTime fecha, List<Turno> agenda)
    {
        return !agenda.Any(t => t.Fecha == fecha);
    }
}

public class Turno
{
    public int Id { get; set; }
    public DateTime Fecha { get; set; }
    public string? Observaciones { get; set; }

    // Composición (tema 02): el turno tiene un paciente y un médico
    public Paciente Paciente { get; set; } = new();
    public Medico Medico { get; set; } = new();

    // [V4] «La clínica atiende de lunes a viernes, de 8 a 20».
    // Alcanza con los datos del propio turno. Vale para cualquier caso de uso
    // que cree o mueva un turno (sacarlo, reprogramarlo…).
    public bool EnHorarioDeAtencion()
    {
        bool esFinDeSemana = Fecha.DayOfWeek is DayOfWeek.Saturday or DayOfWeek.Sunday;
        return !esFinDeSemana && Fecha.Hour >= 8 && Fecha.Hour < 20;
    }
}
