// Las abstracciones de las que dependen los controladores y los servicios.
// Dicen QUÉ se puede hacer; las clases que las implementan dicen CÓMO.

public interface IPacienteRepository
{
    List<Paciente> Listar();
    List<Paciente> Buscar(string texto);
    Paciente? Obtener(int id);
    bool ExisteDni(string dni);
    int Insertar(Paciente paciente);
}

public interface IMedicoRepository
{
    List<Medico> Listar();
    Medico? Obtener(int id);
}

public interface ITurnoRepository
{
    List<Turno> Listar();
    List<Turno> ListarPorPaciente(int pacienteId);
    bool MedicoOcupado(int medicoId, DateTime fecha);
    int Insertar(Turno turno);
}

public interface IUsuarioRepository
{
    Usuario? Obtener(string nombreUsuario);
}
