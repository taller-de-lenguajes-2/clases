// CASO DE USO «registrar un paciente».
public class RegistrarPaciente(PacienteRepository pacientes)
{
    public Resultado Ejecutar(string nombre, string? dni, string? telefono)
    {
        var resultado = new Resultado();

        // ── [V5] REGLA DEL CASO DE USO: no hay dos pacientes con el mismo DNI ──
        // Ningún atributo lo puede saber: hay que consultar la base.
        // (Si igual se cuela un repetido, el UNIQUE de la base lo frena: [V6])
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
