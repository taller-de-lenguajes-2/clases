// Escenario 1: de un modelo complejo a uno simple.
// Una fila del listado de turnos: solo textos, ya formateados para mostrar.
public class TurnoListadoViewModel
{
    public string Dia { get; }          // "mar 20/10"
    public string Hora { get; }         // "09:00"
    public string Paciente { get; }
    public string Medico { get; }       // "Inés Molina (Cardiología)"
    public bool YaPaso { get; }

    public TurnoListadoViewModel(Turno turno)
    {
        Dia = turno.Fecha.ToString("ddd dd/MM");
        Hora = turno.Fecha.ToString("HH:mm");
        Paciente = turno.Paciente.Nombre;
        Medico = turno.Medico.Especialidad is null
            ? turno.Medico.Nombre
            : $"{turno.Medico.Nombre} ({turno.Medico.Especialidad})";
        YaPaso = turno.Fecha < DateTime.Now;
    }
}
