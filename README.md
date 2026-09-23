# Clases — Taller de Lenguajes 2

Material de la cátedra **Taller de Lenguajes 2** (FACET · UNT): presentaciones de cada tema en HTML y, donde corresponda, proyectos de ejemplo ejecutables.

📖 **[Índice de clases online](https://taller-de-lenguajes-2.github.io/clases/index.html)**

## Índice de temas

El material se organiza **por tema** (estable entre años); el orden de dictado de cada año se publica en el [índice online](https://taller-de-lenguajes-2.github.io/clases/index.html).

| Tema | Contenido | Material |
| --- | --- | --- |
| 00 | Sobre la materia (presentación, cursado, evaluación) | [tema-00-sobre-la-materia](./tema-00-sobre-la-materia) |
| 01 | POO: Abstracción y ocultamiento de información | [tema-01-abstraccion-y-ocultamiento](./tema-01-abstraccion-y-ocultamiento) |
| 02 | POO: Composición y agregación | [tema-02-composicion-y-agregacion](./tema-02-composicion-y-agregacion) |
| 03 | POO: Herencia y polimorfismo | [tema-03-herencia-y-polimorfismo](./tema-03-herencia-y-polimorfismo) |
| 04 | POO: Interfaces y clases abstractas | [tema-04-interfaces-y-clases-abstractas](./tema-04-interfaces-y-clases-abstractas) |
| 05 | API REST | [tema-05-api-rest](./tema-05-api-rest) |
| 06 | ASP.NET con API REST | [tema-06-asp-net-web-api](./tema-06-asp-net-web-api) |
| 07 | Bases de datos | _pendiente_ |
| 08 | Bases de datos: ADO.NET | _pendiente_ |
| 09 | Repositorios | _pendiente_ |
| 10 | MVC | _pendiente_ |
| 11 | Layout y Views en ASP.NET | _pendiente_ |
| 12 | ViewModels | _pendiente_ |
| 13 | Validación | _pendiente_ |
| 14 | Inyección de dependencias e interfaces | _pendiente_ |
| 15 | Cookies y variables de sesión | _pendiente_ |
| 16 | Excepciones | [tema-16-excepciones](./tema-16-excepciones) |
| 17 | Logs | [tema-17-logs](./tema-17-logs) |

## Mapeo temas → clases 2026

| Clase | Temas |
| --- | --- |
| 1 | 01 · 02 |
| 2 | 03 · 04 |
| 3 | 05 |
| 4 | 06 |
| 5 | 16 _(excepciones se adelanta este año; logs, tema 17, queda para más adelante)_ |
| 6 | 07 |
| 7 | 08 · 09 |
| 8 | 10 · 11 |
| 9 | 12 · 13 · 15 |
| 10 | 14 |

## Framework de presentaciones

Las presentaciones usan **[slidedeck](https://github.com/Spktro/slidedeck)** (de la propia cátedra) vía CDN de jsDelivr: cada slide es HTML plano dentro de un `<div class="deck">`; el framework aporta tema claro/oscuro, selector de tamaño, navegador de miniaturas, copiado de código y export a PDF.

Sobre eso, [`assets/taller2.css`](./assets/taller2.css) agrega una capa de estilo propia de la cátedra. Varias ideas de diseño de esa capa (portadas hero, tarjetas con acento, jerarquía tipográfica marcada) están **adaptadas de [zarazhangrui/frontend-slides](https://github.com/zarazhangrui/frontend-slides)** — crédito a su autora.

## Estructura

```
tema-XX-nombre/
├── index.html         → presentación (se abre en el navegador)
├── img/               → imágenes de la presentación (si tiene)
└── proyecto-ejemplo/  → proyecto .NET ejecutable (si tiene): dotnet run
```

## Ver una presentación

Abrí el `index.html` del tema en cualquier navegador. Navegación: flechas / espacio / click en las mitades de la slide. Export a PDF: `Ctrl/Cmd + P`.

## Ejecutar los proyectos de ejemplo

Requiere el [.NET SDK](https://dotnet.microsoft.com/download).

```bash
cd tema-XX-nombre/proyecto-ejemplo
dotnet run
```
