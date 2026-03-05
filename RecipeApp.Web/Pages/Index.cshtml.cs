using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RecipeApp.Web.Models;
using RecipeApp.Web.Services;
using System.Collections.Generic;

namespace RecipeApp.Web.Pages
{
    public class IndexModel : PageModel
    {
        private readonly RecipeService _recipeService;
        private readonly CategoryService _categoryService;
        private readonly DifficultyService _difficultyService;

        public IndexModel(
            RecipeService recipeService,
            CategoryService categoryService,
            DifficultyService difficultyService)
        {
            _recipeService = recipeService;
            _categoryService = categoryService;
            _difficultyService = difficultyService;
        }

        public List<Recipe> Recipes { get; set; } = new();
        public List<Category> Categories { get; set; } = new();
        public List<Difficulty> Difficulties { get; set; } = new();

        // Guardar o filtro atual para manter a aba ativa no HTML
        public string CurrentSort { get; set; }

        public void OnGet(string? searchTerm, long? categoryId, long? difficultyId, string sort = "Todas")
        {
            CurrentSort = sort;
            var user = SessionHelper.GetUser(HttpContext);
            long? userId = user?.UserId;

            // 1. Carregar filtros das dropdowns
            Categories = _categoryService.GetAllCategories() ?? new List<Category>();
            Difficulties = _difficultyService.GetAll() ?? new List<Difficulty>();

            // 2. Carregar receitas passando o parâmetro de ordenação (sort)
            // Atualizei o método para aceitar o critério de ordenação das abas
            Recipes = _recipeService.GetHomeRecipes(userId, searchTerm, categoryId, difficultyId, sort) ?? new List<Recipe>();
        }

        public IActionResult OnPostToggleFavorite(long id)
        {
            var user = SessionHelper.GetUser(HttpContext);
            if (user == null) return RedirectToPage("/Login");

            _recipeService.ToggleFavorite(user.UserId, id);
            return RedirectToPage();
        }

        public IActionResult OnPostDelete(long id)
        {
            var user = SessionHelper.GetUser(HttpContext);
            if (user == null) return RedirectToPage("/Login");

            bool isAdmin = SessionHelper.IsAdmin(HttpContext);
            _recipeService.SoftDelete(id, user.UserId, isAdmin);

            TempData["SuccessMessage"] = "Receita removida com sucesso!";
            return RedirectToPage();
        }
    }
}