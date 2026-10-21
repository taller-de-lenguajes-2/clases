# ClinicaApi — Web API con ADO.NET

La API de la clínica (pacientes, médicos y turnos) del tema 07, con los datos en `clinica.db`, una base SQLite.
El acceso a datos es **ADO.NET a mano**: `SqliteConnection`, `SqliteCommand` y `SqliteDataReader`, siempre con parámetros. No usa Entity Framework ni Dapper.

## Cómo correrlo

```bash
cd tema-08-ado-net/proyecto-api
dotnet run
```

Se abre Swagger en `http://localhost:5188/swagger`. Otra forma de probarlo es abrir `ClinicaApi.http` en VS Code (extensión REST Client) o en Visual Studio y mandar los pedidos de a uno.

La primera vez que corre, la API crea `clinica.db` en la carpeta del proyecto con el script del tema 07 (`tema-07-bases-de-datos/material/clinica.sql`).
Los cambios que hagas con POST, PUT y DELETE quedan guardados en esa base. **Para volver a los datos originales, borrá `clinica.db`** y volvé a correr la API.
La cadena de conexión está en `appsettings.json` (`ConnectionStrings:Clinica`).

## Qué hay en cada parte

| Archivo | Qué muestra |
|---|---|
| `Program.cs` | Lee la cadena de conexión, crea la base si no existe y registra los repositorios y Swagger |
| `Modelos.cs` | Una clase por tabla, más `TurnoDetalle`, que es la forma de cada fila que devuelve el JOIN |
| `Repositorios/` | **Todo el SQL.** Un repositorio por tabla: `Listar`, `Obtener`, `Insertar` (`INSERT … RETURNING Id`), `Actualizar`, `Eliminar` |
| `Controllers/` | Reciben los pedidos HTTP, llaman al repositorio y eligen el código de respuesta. Ningún controlador escribe SQL |
| `ClinicaApi.http` | Un pedido de ejemplo por endpoint, con el código de respuesta que tiene que devolver |

## Endpoints

| Verbo | Ruta | Respuesta |
|---|---|---|
| GET | `api/pacientes` · `api/medicos` · `api/turnos` | 200 con la lista |
| GET | `api/pacientes/{id}` (lo mismo para médicos y turnos) | 200 · 404 |
| POST | `api/pacientes` (lo mismo para médicos y turnos) | 201 + `Location` · 400 |
| PUT | `api/pacientes/{id}` (lo mismo para médicos y turnos) | 204 · 404 · 400 |
| DELETE | `api/pacientes/{id}` (lo mismo para médicos y turnos) | 204 · 404 · 409 si tiene turnos |
| GET | `api/pacientes/{id}/turnos` | JOIN: los turnos del paciente, con el nombre del médico |
| GET | `api/turnos?fecha=2026-10-20` | JOIN: los turnos de ese día |

## Para mirar en clase

- **Parámetros:** todo el SQL usa `@id`, `@nombre`, etc. y `Parameters.AddWithValue`. Nunca se arma el SQL concatenando lo que manda el usuario.
- **NULL:** al escribir, un `null` de C# se manda como `DBNull.Value`. Al leer, `lector["Telefono"] as string` devuelve `null` si la columna es NULL.
- **ExecuteScalar, ExecuteNonQuery y ExecuteReader:** el INSERT devuelve el Id nuevo. UPDATE y DELETE devuelven cuántas filas cambiaron: si son 0, el registro no existe y la API responde 404.
- **Restricciones de la base:** la base rechaza un DNI repetido (`UNIQUE`) y no deja borrar un paciente o un médico que tiene turnos (`FOREIGN KEY`). En los dos casos, SQLite tira una `SqliteException` con el código 19 y el controlador responde 409.
- **Fechas:** la base guarda la fecha como texto con el formato `'AAAA-MM-DD HH:MM'`. El repositorio la convierte desde y hacia `DateTime`.
