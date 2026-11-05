// La cadena de conexión envuelta en un tipo propio.
// El contenedor busca las dependencias por TIPO: si un repositorio pidiera un string a secas,
// no sabría cuál darle. Con ConexionDb no hay dudas, y el repositorio recibe solo lo que usa.
// Program.cs la crea una vez al arrancar y la registra como Singleton (es un dato que no cambia).
public record ConexionDb(string Cadena);
