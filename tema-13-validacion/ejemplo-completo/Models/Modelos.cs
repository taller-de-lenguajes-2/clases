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

    // ── [V4] REGLA DE NEGOCIO DE LA ENTIDAD ─────────────────────────────
    // «La clínica atiende de lunes a viernes, de 8 a 20».
    // Para saberlo alcanza con los datos del propio turno: no hace falta la base
    // ni saber quién lo pide. Por eso vive en la entidad, y vale para cualquier
    // caso de uso que cree o mueva un turno (sacarlo, reprogramarlo…).
    public bool EnHorarioDeAtencion()
    {
        bool esFinDeSemana = Fecha.DayOfWeek is DayOfWeek.Saturday or DayOfWeek.Sunday;
        return !esFinDeSemana && Fecha.Hour >= 8 && Fecha.Hour < 20;
    }
}
