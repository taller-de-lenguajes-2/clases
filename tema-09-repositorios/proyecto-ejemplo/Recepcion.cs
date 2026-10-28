// La "lógica de negocio" de la recepción de la clínica.
// Recibe el repositorio en el constructor y solo conoce la interfaz:
// no sabe si los pacientes están en SQLite, en un JSON o en otro lado.
public class Recepcion(IPacienteRepository pacientes)
{
    public void MostrarPacientes()
    {
        foreach (var p in pacientes.Listar())
            Console.WriteLine($"{p.Id}  {p.Nombre,-18} tel: {p.Telefono ?? "(sin teléfono)"}");
    }

    public void MostrarBusqueda(string texto)
    {
        var encontrados = pacientes.Buscar(texto);
        Console.WriteLine($"«{texto}»: {encontrados.Count} paciente(s)");
        foreach (var p in encontrados)
            Console.WriteLine($"   {p.Nombre}");
    }

    // Una regla del negocio: no se da de alta un DNI que ya está cargado
    public int? DarDeAlta(Paciente nuevo)
    {
        bool repetido = nuevo.DNI is not null && pacientes.Listar().Any(p => p.DNI == nuevo.DNI);
        if (repetido)
        {
            Console.WriteLine($"No se da de alta a {nuevo.Nombre}: el DNI {nuevo.DNI} ya existe.");
            return null;
        }

        int id = pacientes.Insertar(nuevo);
        Console.WriteLine($"Alta de {nuevo.Nombre} con Id {id}.");
        return id;
    }

    public void CambiarTelefono(int id, string telefono)
    {
        var paciente = pacientes.Obtener(id);
        if (paciente is null)
        {
            Console.WriteLine($"No existe el paciente {id}.");
            return;
        }

        paciente.Telefono = telefono;
        pacientes.Actualizar(paciente);
        Console.WriteLine($"{paciente.Nombre}: teléfono nuevo {telefono}.");
    }

    public void DarDeBaja(int id) =>
        Console.WriteLine(pacientes.Eliminar(id) ? $"Baja del paciente {id}." : $"No existe el paciente {id}.");
}
