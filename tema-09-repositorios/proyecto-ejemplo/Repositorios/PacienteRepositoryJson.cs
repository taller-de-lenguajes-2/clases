using System.Text.Json;

// Otra implementación del mismo contrato: los pacientes en un archivo JSON.
// Quien use IPacienteRepository no se entera de la diferencia.
public class PacienteRepositoryJson(string archivo) : IPacienteRepository
{
    private static readonly JsonSerializerOptions Opciones = new() { WriteIndented = true };

    public List<Paciente> Listar() => Leer().OrderBy(p => p.Nombre).ToList();

    public Paciente? Obtener(int id) => Leer().FirstOrDefault(p => p.Id == id);

    public List<Paciente> Buscar(string texto) =>
        Leer().Where(p => p.Nombre.Contains(texto, StringComparison.OrdinalIgnoreCase))
              .OrderBy(p => p.Nombre)
              .ToList();

    public int Insertar(Paciente paciente)
    {
        var pacientes = Leer();
        paciente.Id = pacientes.Count == 0 ? 1 : pacientes.Max(p => p.Id) + 1;   // el Id lo generamos nosotros
        pacientes.Add(paciente);
        Guardar(pacientes);
        return paciente.Id;
    }

    public bool Actualizar(Paciente paciente)
    {
        var pacientes = Leer();
        int posicion = pacientes.FindIndex(p => p.Id == paciente.Id);
        if (posicion == -1) return false;

        pacientes[posicion] = paciente;
        Guardar(pacientes);
        return true;
    }

    public bool Eliminar(int id)
    {
        var pacientes = Leer();
        bool borrado = pacientes.RemoveAll(p => p.Id == id) > 0;
        if (borrado) Guardar(pacientes);
        return borrado;
    }

    // Leer y escribir el archivo entero en cada operación: los límites del tema 07
    private List<Paciente> Leer() =>
        File.Exists(archivo)
            ? JsonSerializer.Deserialize<List<Paciente>>(File.ReadAllText(archivo)) ?? []
            : [];

    private void Guardar(List<Paciente> pacientes) =>
        File.WriteAllText(archivo, JsonSerializer.Serialize(pacientes, Opciones));
}
