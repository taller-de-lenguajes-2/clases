// CASO DE USO «sacar un turno»: lo que hace la aplicación, paso a paso.
// Coordina a las entidades y a los repositorios; no sabe nada de HTML ni de formularios.
//
// Acá van las reglas de negocio que necesitan OTROS datos además del turno
// (si el paciente existe, si el médico ya está ocupado): la entidad sola no las puede
// saber, porque para eso hay que consultar la base.
public class SacarTurno(TurnoRepository turnos, PacienteRepository pacientes, MedicoRepository medicos)
{
    public Resultado Ejecutar(int pacienteId, int medicoId, DateTime fecha, string? observaciones)
    {
        var resultado = new Resultado();

        // ── [V5] REGLA DEL CASO DE USO: el paciente y el médico tienen que existir ──
        // El formulario manda solo un número: alguien puede mandar un Id que no está.
        Paciente? paciente = pacientes.Obtener(pacienteId);
        Medico? medico = medicos.Obtener(medicoId);

        if (paciente is null)
            resultado.AgregarError("PacienteId", "Ese paciente no existe.");
        if (medico is null)
            resultado.AgregarError("MedicoId", "Ese médico no existe.");
        if (paciente is null || medico is null)
            return resultado;

        var turno = new Turno
        {
            Fecha = fecha,
            Observaciones = observaciones,
            Paciente = paciente,
            Medico = medico
        };

        // ── [V4] REGLA DE LA ENTIDAD: el caso de uso la usa, pero no sabe cómo se calcula ──
        if (!turno.EnHorarioDeAtencion())
        {
            resultado.AgregarError("Fecha", "La clínica atiende de lunes a viernes, de 8 a 20.");
            return resultado;
        }

        // ── [V5] REGLA DEL CASO DE USO: un médico no atiende dos turnos a la misma hora ──
        // Necesita los otros turnos del médico: hay que preguntarle a la base.
        if (turnos.MedicoOcupado(medicoId, fecha))
        {
            resultado.AgregarError("Fecha", $"{medico.Nombre} ya tiene un turno a esa hora.");
            return resultado;
        }

        // Todas las reglas se cumplen: recién ahora se guarda
        resultado.Id = turnos.Insertar(turno);
        return resultado;
    }
}
