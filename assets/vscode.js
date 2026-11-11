/* ============================================================
   Botones «Abrir en VS Code» (complemento de slidedeck)

   Uso: cualquier elemento con data-vsc="ruta:línea", con la ruta relativa
   a la raíz del repo «clases». Al hacer clic abre ese archivo (o carpeta)
   en Visual Studio Code con el enlace vscode://file/…

     <a class="vsc" href="#" data-vsc="tema-13-validacion/ejemplo-completo">…</a>
     <a class="vsc" href="#" data-vsc="tema-13-validacion/ejemplo-completo/Models/Modelos.cs:29">…</a>

   ¿Dónde está el repo en esta computadora?
   - Si la presentación se abrió como archivo (file://), lo saca de la URL.
   - Si no (GitHub Pages, un servidor local), lo pregunta la primera vez y
     lo recuerda en este navegador. Alt + clic en un botón lo vuelve a preguntar.

   Abrir primero la carpeta del proyecto: los archivos se abren después en
   esa misma ventana de VS Code.
   ============================================================ */
(function () {
  const CLAVE = "taller2-ruta-repo";

  const estilo = document.createElement("style");
  estilo.textContent = `
    .vsc { display: inline-flex; align-items: center; gap: 6px; padding: 3px 10px; margin: 2px 4px 2px 0;
           border: 1px solid color-mix(in srgb, #007acc 45%, transparent); border-radius: 999px;
           background: color-mix(in srgb, #007acc 10%, transparent); color: inherit;
           font: 600 14px/1.4 "JetBrains Mono", ui-monospace, monospace; text-decoration: none; cursor: pointer; }
    .vsc::before { content: ""; width: 11px; height: 11px; border-radius: 2px; background: #007acc; flex: none; }
    .vsc:hover { background: color-mix(in srgb, #007acc 22%, transparent); text-decoration: none; }
    .vsc.proyecto { font-size: 16px; padding: 6px 14px; background: #007acc; color: #fff; border-color: #007acc; }
    .vsc.proyecto::before { background: #fff; }
    .vsc-linea { color: var(--muted, #777); font-weight: 400; }
    @media print { .vsc { border-color: #999; background: none; color: inherit; } }
  `;
  document.head.appendChild(estilo);

  function raizDesdeArchivo() {
    if (location.protocol !== "file:") return null;
    const ruta = decodeURIComponent(location.pathname);
    const i = ruta.lastIndexOf("/tema-");
    return i > 0 ? ruta.slice(0, i) : null;
  }

  function leer() { try { return localStorage.getItem(CLAVE); } catch { return null; } }
  function guardar(v) { try { localStorage.setItem(CLAVE, v); } catch { /* sin almacenamiento */ } }

  function raiz(volverAPreguntar) {
    const desdeArchivo = raizDesdeArchivo();
    if (desdeArchivo && !volverAPreguntar) return desdeArchivo;
    let r = leer();
    if (!r || volverAPreguntar) {
      r = prompt("¿En qué carpeta está el repo «clases» en esta computadora?\n(ruta completa, por ejemplo /Users/ana/clases o C:\\Users\\ana\\clases)", r || desdeArchivo || "");
      if (!r) return null;
      r = r.trim().replace(/\\/g, "/").replace(/\/+$/, "");
      guardar(r);
    }
    return r;
  }

  function enlace(base, relativo) {
    const [archivo, linea] = relativo.split(":");
    let ruta = (base + "/" + archivo).replace(/\\/g, "/");
    if (!ruta.startsWith("/")) ruta = "/" + ruta;          // C:/… → /C:/…
    return "vscode://file" + encodeURI(ruta) + (linea ? ":" + linea + ":1" : "");
  }

  document.addEventListener("click", (e) => {
    const boton = e.target.closest("[data-vsc]");
    if (!boton || boton.closest(".thumb")) return;        // las miniaturas no abren nada
    e.preventDefault();
    e.stopPropagation();
    const base = raiz(e.altKey);
    if (base) location.href = enlace(base, boton.dataset.vsc);
  }, true);

  // Si el botón no tiene texto, muestra el nombre del archivo y la línea
  document.querySelectorAll("[data-vsc]").forEach((b) => {
    if (!b.getAttribute("href")) b.setAttribute("href", "#");
    if (!b.title) b.title = "Abrir en VS Code: " + b.dataset.vsc;
    if (b.textContent.trim()) return;
    const [archivo, linea] = b.dataset.vsc.split(":");
    b.textContent = archivo.split("/").pop();
    if (linea) { const s = document.createElement("span"); s.className = "vsc-linea"; s.textContent = ":" + linea; b.appendChild(s); }
  });
})();
