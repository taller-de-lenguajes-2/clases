// CASO DE USO «sacar un turno»: lo que hace la aplicación, paso a paso.
//
// ── [V5] REGLAS DE LA APLICACIÓN ────────────────────────────────────────
// Existen porque el proceso está automatizado: qué buscar, en qué orden,
// qué hacer si algo falla y cuándo guardar.
// El caso de uso CONSIGUE los datos (de la base) y le PREGUNTA a las entidades;
// las reglas del negocio no las decide él [V4]. No sabe nada de HTML ni de formularios.
public class SacarTurno(TurnoRepository turnos, PacienteRepository pacientes, MedicoRepository medicos)
{
    public Resultado Ejecutar(int pacienteId, int medicoId, DateTime fecha, string? observaciones)
    {
        var resultado = new Resultado();

        // 1 · Buscar el paciente y el médico. El formulario manda solo un número:
        //     si alguno no existe, el proceso no sigue.                         [V5]
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

        // 2 · Preguntarle al turno si está en horario.                         [V4]
        if (!turno.EnHorarioDeAtencion())
        {
            resultado.AgregarError("Fecha", "La clínica atiende de lunes a viernes, de 8 a 20.");
            return resultado;
        }

        // 3 · Traer la agenda del médico (la base) y preguntarle si está libre. [V5] → [V4]
        List<Turno> agenda = turnos.ListarPorMedico(medicoId);
        if (!medico.EstaLibre(fecha, agenda))
        {
            resultado.AgregarError("Fecha", $"{medico.Nombre} ya tiene un turno a esa hora.");
            return resultado;
        }

        // 4 · Todas las reglas se cumplen: recién ahora se guarda.              [V5]
        resultado.Id = turnos.Insertar(turno);
        return resultado;
    }
}
