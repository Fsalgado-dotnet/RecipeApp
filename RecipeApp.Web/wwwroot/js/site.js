/**
 * RecipeApp - Main JavaScript
 */

document.addEventListener("DOMContentLoaded", function () {

    // 1. GESTÃO DE FILTROS (Index Page)
    const filterForm = document.getElementById('filterForm');
    if (filterForm) {
        const autoFilters = filterForm.querySelectorAll('select');
        const searchInput = filterForm.querySelector('input[name="searchTerm"]');

        // Submeter automaticamente ao mudar Categoria ou Dificuldade
        autoFilters.forEach(filter => {
            filter.addEventListener('change', () => {
                applyLoadingEffect(filterForm);
                filterForm.submit();
            });
        });

        // Feedback visual ao pressionar Enter na pesquisa
        searchInput.addEventListener('keypress', (e) => {
            if (e.key === 'Enter') {
                applyLoadingEffect(filterForm);
            }
        });
    }

    // 2. ANIMAÇÃO DE REVELAÇÃO DOS CARDS (Scroll Reveal)
    const cards = document.querySelectorAll('.recipe-card');
    cards.forEach((card, index) => {
        card.style.opacity = '0';
        card.style.transform = 'translateY(20px)';
        card.style.transition = 'all 0.5s ease forwards';

        setTimeout(() => {
            card.style.opacity = '1';
            card.style.transform = 'translateY(0)';
        }, 80 * index);
    });

    // 3. UTILITÁRIOS
    function applyLoadingEffect(element) {
        element.style.opacity = '0.6';
        element.style.pointerEvents = 'none';
    }

    // 4. TOOLTIPS (Se usares Bootstrap Tooltips)
    var tooltipTriggerList = [].slice.call(document.querySelectorAll('[data-bs-toggle="tooltip"]'))
    var tooltipList = tooltipTriggerList.map(function (tooltipTriggerEl) {
        return new bootstrap.Tooltip(tooltipTriggerEl)
    });

    console.log("RecipeApp: Scripts carregados com sucesso!");
});