// El contrato: QUÉ se puede hacer con los pacientes guardados.
// No dice CÓMO ni DÓNDE se guardan (eso lo decide cada implementación).
public interface IPacienteRepository
{
    List<Paciente> Listar();
    Paciente? Obtener(int id);              // null si no existe
    List<Paciente> Buscar(string texto);    // por nombre
    int Insertar(Paciente paciente);        // devuelve el Id nuevo
    bool Actualizar(Paciente paciente);     // false si no existe
    bool Eliminar(int id);                  // false si no existe
}
