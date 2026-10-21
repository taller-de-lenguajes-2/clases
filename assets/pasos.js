/* ============================================================
   Pasos dentro de una slide (complemento de slidedeck)

   Uso: la slide lleva el atributo data-pasos y cada elemento que
   aparece de a poco lleva data-paso="n" (1, 2, 3…). Con → / espacio /
   PageDown, o con un clic sobre el .diagram, se muestra el paso
   siguiente; recién cuando no quedan pasos, → pasa a la otra slide.
   ← retrocede un paso. Al entrar a la slide arranca en el paso 0.

   Marco: un <div class="marco"> dentro de un contenedor .marco-caja
   lleva data-marco-n="selector" (y opcional data-etiqueta-n="texto").
   En el paso n rodea a los elementos del contenedor que coinciden con
   el selector; en los pasos sin selector queda oculto. Con data-marco
   (sin número) rodea siempre a lo mismo, en todos los pasos.
   Flecha: <svg class="marco-flecha" data-desde=".m1" data-hasta=".m2">
   en la misma caja une dos marcos; se dibuja de nuevo en cada paso.

   Imagen ampliada (lightbox): un clic sobre cualquier <img> de una
   slide la agranda al centro, sobre fondo oscuro. Otro clic, un clic
   en el fondo o Escape la cierran. Mientras está abierta, las teclas no
   cambian de slide ni de paso. Para excluir una imagen: data-sin-zoom
   o la clase sin-zoom (los íconos .icon-tile y las imágenes dentro de
   un link ya quedan excluidos).
   ============================================================ */

// --- Imagen ampliada ---------------------------------------------------
// Va antes que los pasos: su keydown en captura tiene que correr primero.
(function () {
  const deck = document.getElementById("deck");
  if (!deck) return;
  const EXCLUIDAS = "[data-sin-zoom], .sin-zoom, .icon-tile img, a img";
  const ampliable = (e) => e instanceof HTMLImageElement && deck.contains(e) && !e.matches(EXCLUIDAS);
  const sinAnimacion = () => window.matchMedia("(prefers-reduced-motion: reduce)").matches;
  const DURACION = 350;

  let abierta = null; // { original, fondo, copia }

  const rectDe = (r) => ({ left: `${r.left}px`, top: `${r.top}px`, width: `${r.width}px`, height: `${r.height}px` });
  const centrado = (img) => {
    const r = img.getBoundingClientRect();
    const ancho = img.naturalWidth || r.width, alto = img.naturalHeight || r.height;
    // Entra en el 92 % × 90 % de la pantalla, sin pasar del doble de su tamaño real
    const escala = Math.min((innerWidth * 0.92) / ancho, (innerHeight * 0.9) / alto, 2);
    const w = ancho * escala, h = alto * escala;
    return { left: `${(innerWidth - w) / 2}px`, top: `${(innerHeight - h) / 2}px`, width: `${w}px`, height: `${h}px` };
  };

  const abrir = (img) => {
    const fondo = document.createElement("div");
    fondo.className = "lightbox-fondo";
    const copia = img.cloneNode();
    copia.removeAttribute("style");
    copia.removeAttribute("class");
    copia.className = "lightbox-img";
    Object.assign(copia.style, rectDe(img.getBoundingClientRect()));
    document.body.append(fondo, copia);
    img.style.visibility = "hidden";
    abierta = { original: img, fondo, copia };
    void copia.offsetWidth; // que arranque desde el lugar de la imagen
    fondo.classList.add("abierto");
    Object.assign(copia.style, centrado(img));
  };

  const cerrar = () => {
    if (!abierta) return;
    const { original, fondo, copia } = abierta;
    abierta = null;
    fondo.classList.remove("abierto");
    Object.assign(copia.style, rectDe(original.getBoundingClientRect()));
    setTimeout(() => {
      fondo.remove();
      copia.remove();
      original.style.visibility = "";
    }, sinAnimacion() ? 0 : DURACION);
  };

  // En captura: el clic no llega al .diagram (que avanzaría un paso)
  document.addEventListener("click", (e) => {
    if (abierta) {
      e.preventDefault();
      e.stopPropagation();
      cerrar();
    } else if (ampliable(e.target)) {
      e.preventDefault();
      e.stopPropagation();
      abrir(e.target);
    }
  }, true);

  // Con la imagen abierta, ninguna tecla llega al deck ni a los pasos
  window.addEventListener("keydown", (e) => {
    if (!abierta) return;
    e.preventDefault();
    e.stopImmediatePropagation();
    if (e.key === "Escape" || e.key === "Enter" || e.key === " ") cerrar();
  }, true);

  window.addEventListener("resize", () => { if (abierta) Object.assign(abierta.copia.style, centrado(abierta.original)); });
})();

// --- Pasos y marcos ----------------------------------------------------
(function () {
  const deck = document.getElementById("deck");
  if (!deck) return;
  const slides = Array.from(deck.querySelectorAll(":scope > .slide[data-pasos]"));
  if (!slides.length) return;

  const elementos = (s) => Array.from(s.querySelectorAll("[data-paso]"));
  const marcos = (s) => Array.from(s.querySelectorAll(".marco"));
  const pasosDelMarco = (m) => m.getAttributeNames()
    .map((a) => /^data-marco-(\d+)$/.exec(a)).filter(Boolean).map((r) => +r[1]);
  const ultimo = (s) => Math.max(0, ...elementos(s).map((e) => +e.dataset.paso), ...marcos(s).flatMap(pasosDelMarco));
  const actual = (s) => +(s.dataset.pasoActual || 0);

  // Posición relativa a la caja con offset* (no la altera la escala del deck)
  const posicion = (e, caja) => {
    let x = 0, y = 0;
    for (let a = e; a && a !== caja; a = a.offsetParent) { x += a.offsetLeft; y += a.offsetTop; }
    return { x, y, x2: x + e.offsetWidth, y2: y + e.offsetHeight };
  };
  const ubicar = (m, n) => {
    const caja = m.closest(".marco-caja");
    const selector = m.getAttribute(`data-marco-${n}`) ?? m.getAttribute("data-marco");
    const objetivos = caja && selector ? Array.from(caja.querySelectorAll(selector)) : [];
    m._rect = null;
    if (!objetivos.length) { m.classList.remove("visto"); return; }
    const r = objetivos.map((e) => posicion(e, caja)).reduce((a, b) => ({
      x: Math.min(a.x, b.x), y: Math.min(a.y, b.y), x2: Math.max(a.x2, b.x2), y2: Math.max(a.y2, b.y2),
    }));
    // Slide todavía oculta: no hay qué medir, se ubica cuando se vea
    if (r.x2 - r.x <= 0) { m.classList.remove("visto"); return; }
    // Para las flechas: dónde quedó el marco y la tabla que lo contiene
    const tabla = objetivos[0].closest("table");
    m._rect = r;
    m._tabla = tabla && caja.contains(tabla) ? posicion(tabla, caja) : null;
    const margen = 4;
    const aparece = !m.classList.contains("visto");
    if (aparece) m.style.transition = "none";
    Object.assign(m.style, {
      left: `${r.x - margen}px`, top: `${r.y - margen}px`,
      width: `${r.x2 - r.x + 2 * margen}px`, height: `${r.y2 - r.y + 2 * margen}px`,
    });
    const etiqueta = m.querySelector(".marco-etiqueta");
    const texto = m.getAttribute(`data-etiqueta-${n}`) ?? m.getAttribute("data-etiqueta");
    if (etiqueta && texto !== null) etiqueta.textContent = texto;
    if (aparece) { void m.offsetWidth; m.style.transition = ""; }
    m.classList.add("visto");
  };

  // Flecha entre dos marcos: <svg class="marco-flecha" data-desde=".a" data-hasta=".b">
  // Sale del borde derecho de la tabla del primero y llega al borde izquierdo del segundo.
  const SVG = "http://www.w3.org/2000/svg";
  const ubicarFlechas = (s, redibujar) => s.querySelectorAll(".marco-flecha").forEach((f) => {
    const caja = f.closest(".marco-caja");
    const a = caja?.querySelector(f.dataset.desde), b = caja?.querySelector(f.dataset.hasta);
    if (!a?._rect || !b?._rect || !a.classList.contains("visto") || !b.classList.contains("visto")) {
      f.classList.remove("visto", "dibujando");
      return;
    }
    if (!f.firstChild) {
      const linea = document.createElementNS(SVG, "path");
      linea.setAttribute("class", "linea");
      linea.setAttribute("pathLength", "1");
      const punta = document.createElementNS(SVG, "path");
      punta.setAttribute("class", "punta");
      f.append(linea, punta);
    }
    const x1 = (a._tabla ? a._tabla.x2 : a._rect.x2) + 8, y1 = (a._rect.y + a._rect.y2) / 2;
    const x2 = b._rect.x - 10, y2 = (b._rect.y + b._rect.y2) / 2;
    const curva = Math.max(30, (x2 - x1) / 2);
    f.querySelector(".linea").setAttribute("d", `M${x1},${y1} C${x1 + curva},${y1} ${x2 - curva},${y2} ${x2 - 2},${y2}`);
    f.querySelector(".punta").setAttribute("d", `M${x2 - 12},${y2 - 7} L${x2},${y2} L${x2 - 12},${y2 + 7} z`);
    f.classList.add("visto");
    if (redibujar) { f.classList.remove("dibujando"); void f.getBoundingClientRect(); f.classList.add("dibujando"); }
  });

  const mostrar = (s, n) => {
    s.dataset.pasoActual = n;
    elementos(s).forEach((e) => e.classList.toggle("visto", +e.dataset.paso <= n));
    marcos(s).forEach((m) => ubicar(m, n));
    ubicarFlechas(s, true);
  };
  const activa = () => slides.find((s) => s.classList.contains("active"));

  // En captura, para decidir antes que el deck si la tecla es un paso o un cambio de slide
  window.addEventListener("keydown", (e) => {
    const s = activa();
    if (!s) return;
    const n = actual(s);
    let destino = null;
    if ((e.key === "ArrowRight" || e.key === "PageDown" || e.key === " ") && n < ultimo(s)) destino = n + 1;
    else if ((e.key === "ArrowLeft" || e.key === "PageUp") && n > 0) destino = n - 1;
    if (destino === null) return;
    mostrar(s, destino);
    e.preventDefault();
    e.stopImmediatePropagation();
  }, true);

  // Clic sobre el diagrama: avanza un paso
  slides.forEach((s) => {
    s.querySelectorAll(".diagram").forEach((d) => {
      d.style.cursor = "pointer";
      d.addEventListener("click", () => {
        if (s.classList.contains("active") && actual(s) < ultimo(s)) mostrar(s, actual(s) + 1);
      });
    });
    // Al entrar a la slide, vuelve al paso 0
    let estabaActiva = s.classList.contains("active");
    new MutationObserver(() => {
      const ahora = s.classList.contains("active");
      if (ahora && !estabaActiva) {
        mostrar(s, 0);
        // Los marcos se miden cuando la slide ya está visible y acomodada
        requestAnimationFrame(() => { marcos(s).forEach((m) => ubicar(m, actual(s))); ubicarFlechas(s, false); });
      }
      estabaActiva = ahora;
    }).observe(s, { attributes: true, attributeFilter: ["class"] });
    mostrar(s, 0);
  });

  // Recién con las fuentes cargadas las tablas tienen su ancho final
  const reubicar = () => slides.forEach((s) => { marcos(s).forEach((m) => ubicar(m, actual(s))); ubicarFlechas(s, false); });
  window.addEventListener("load", reubicar);
  window.addEventListener("resize", reubicar);
  if (document.fonts) document.fonts.ready.then(reubicar);
  // Si la tabla cambia de tamaño (p. ej. con el selector de tamaño del deck), el marco se vuelve a medir
  if (window.ResizeObserver) {
    const observador = new ResizeObserver(reubicar);
    slides.forEach((s) => s.querySelectorAll(".marco-caja").forEach((c) => observador.observe(c)));
  }
})();
