// CASO DE USO «registrar un paciente».
public class RegistrarPaciente(PacienteRepository pacientes)
{
    public Resultado Ejecutar(string nombre, string? dni, string? telefono)
    {
        var resultado = new Resultado();

        // «No hay dos pacientes con el mismo DNI» es una regla del negocio (con fichas de
        // papel tampoco se abrían dos para la misma persona), pero es sobre TODOS los
        // pacientes: ninguna entidad sola la puede decidir.
        // La garantiza la base (UNIQUE) [V6]; el caso de uso pregunta antes de guardar
        // para avisarle al usuario con un mensaje claro.                        [V5]
        if (!string.IsNullOrEmpty(dni) && pacientes.ExisteDni(dni))
        {
            resultado.AgregarError("DNI", "Ya hay un paciente con ese DNI.");
            return resultado;
        }

        var paciente = new Paciente
        {
            Nombre = nombre,
            DNI = dni,
            Telefono = telefono
        };
        resultado.Id = pacientes.Insertar(paciente);
        return resultado;
    }
}
