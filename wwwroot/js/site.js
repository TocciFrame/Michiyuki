// Small progressive-enhancement script: fade sections in on scroll.
// Add class="reveal" to any element. If JS/IntersectionObserver is unavailable, it stays visible.
(function () {
    "use strict";
    function initReveal() {
        var items = document.querySelectorAll(".reveal");
        if (!items.length || !("IntersectionObserver" in window)) return;
        var observer = new IntersectionObserver(function (entries) {
            entries.forEach(function (e) {
                if (e.isIntersecting) {
                    e.target.classList.remove("reveal-init");
                    e.target.classList.add("in-view");
                    observer.unobserve(e.target);
                }
            });
        }, { threshold: 0.12 });
        items.forEach(function (el) { el.classList.add("reveal-init"); observer.observe(el); });
    }
    document.addEventListener("DOMContentLoaded", initReveal);
    // Re-run after Blazor enhanced navigation
    if (window.Blazor) { window.Blazor.addEventListener("enhancedload", initReveal); }
})();
