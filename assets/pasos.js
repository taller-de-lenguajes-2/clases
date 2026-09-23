/* ============================================================
   Pasos dentro de una slide (complemento de slidedeck)

   Uso: la slide lleva el atributo data-pasos y cada elemento que
   aparece de a poco lleva data-paso="n" (1, 2, 3…). Con → / espacio /
   PageDown, o con un clic sobre el .diagram, se muestra el paso
   siguiente; recién cuando no quedan pasos, → pasa a la otra slide.
   ← retrocede un paso. Al entrar a la slide arranca en el paso 0.
   ============================================================ */
(function () {
  const deck = document.getElementById("deck");
  if (!deck) return;
  const slides = Array.from(deck.querySelectorAll(":scope > .slide[data-pasos]"));
  if (!slides.length) return;

  const elementos = (s) => Array.from(s.querySelectorAll("[data-paso]"));
  const ultimo = (s) => Math.max(0, ...elementos(s).map((e) => +e.dataset.paso));
  const actual = (s) => +(s.dataset.pasoActual || 0);
  const mostrar = (s, n) => {
    s.dataset.pasoActual = n;
    elementos(s).forEach((e) => e.classList.toggle("visto", +e.dataset.paso <= n));
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
      if (ahora && !estabaActiva) mostrar(s, 0);
      estabaActiva = ahora;
    }).observe(s, { attributes: true, attributeFilter: ["class"] });
    mostrar(s, 0);
  });
})();
