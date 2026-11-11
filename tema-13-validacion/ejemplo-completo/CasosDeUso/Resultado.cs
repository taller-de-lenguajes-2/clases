// Lo que devuelve un caso de uso: si salió bien, el Id de lo que creó;
// si no, la lista de reglas que no se cumplieron.
// El caso de uso no sabe nada de ModelState ni de vistas: eso lo traduce el controlador.
public class Resultado
{
    public List<ErrorDeRegla> Errores { get; } = [];
    public bool Ok => Errores.Count == 0;
    public int Id { get; set; }

    // "campo" es el nombre del dato que falló (Fecha, DNI…): el controlador lo usa
    // para mostrar el mensaje al lado del campo correspondiente del formulario.
    public void AgregarError(string campo, string mensaje)
    {
        Errores.Add(new ErrorDeRegla(campo, mensaje));
    }
}

public class ErrorDeRegla(string campo, string mensaje)
{
    public string Campo { get; } = campo;
    public string Mensaje { get; } = mensaje;
}
