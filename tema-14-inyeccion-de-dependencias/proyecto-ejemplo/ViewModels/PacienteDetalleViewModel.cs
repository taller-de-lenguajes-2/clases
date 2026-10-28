// Escenario 2: dos modelos (un Paciente y sus Turnos) para una sola vista.
public class PacienteDetalleViewModel
{
    public int Id { get; }
    public string Nombre { get; }
    public string DNI { get; }
    public string Telefono { get; }
    public List<TurnoListadoViewModel> ProximosTurnos { get; }
    public int TurnosAnteriores { get; }

    public PacienteDetalleViewModel(Paciente paciente, List<Turno> turnos)
    {
        Id = paciente.Id;
        Nombre = paciente.Nombre;
        DNI = paciente.DNI ?? "sin DNI";
        Telefono = paciente.Telefono ?? "sin teléfono";

        ProximosTurnos = turnos
            .Where(t => t.Fecha >= DateTime.Now)
            .Select(t => new TurnoListadoViewModel(t))
            .ToList();
        TurnosAnteriores = turnos.Count(t => t.Fecha < DateTime.Now);
    }
}
