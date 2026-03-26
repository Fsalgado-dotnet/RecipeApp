using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using RecipeApp.Models;
using RecipeApp.Services;
using System.Collections.Generic;
using System.Linq;

namespace RecipeApp.Web.Pages
{
    public class CreateRecipeModel : PageModel
    {
        private readonly RecipeService _recipeService;
        private readonly CategoryService _categoryService;
        private readonly DifficultyService _difficultyService;

        public CreateRecipeModel(
            RecipeService recipeService,
            CategoryService categoryService,
            DifficultyService difficultyService)
        {
            _recipeService = recipeService;
            _categoryService = categoryService;
            _difficultyService = difficultyService;
        }

        [BindProperty]
        public Recipe Recipe { get; set; } = new();

        public List<SelectListItem> CategoryOptions { get; set; } = new();
        public List<SelectListItem> DifficultyOptions { get; set; } = new();

        public IActionResult OnGet()
        {
            // Verifica se o utilizador está logado usando o Helper que movi para os Services
            if (!SessionHelper.IsLoggedIn(HttpContext))
                return RedirectToPage("/Login");

            LoadDropdowns();
            return Page();
        }

        public IActionResult OnPost()
        {
            var user = SessionHelper.GetUser(HttpContext);
            if (user == null) return RedirectToPage("/Login");

            // Validação simples para garantir que temos dados mínimos
            if (!ModelState.IsValid)
            {
                LoadDropdowns();
                return Page();
            }

            // 1. Criamos a receita e guardamos o ID gerado pelo banco de dados
            
            int newRecipeId = _recipeService.CreateRecipe(Recipe, user.UserId);

            // 2. Mensagem de feedback para o utilizador
            TempData["SuccessMessage"] = "Informações básicas gravadas! Adicione agora os ingredientes.";

            // 3. Redirecionamos para a página de ingredientes, passando o ID da nova receita
            // Isso resolve o problema de saltar a etapa dos ingredientes
            return RedirectToPage("AddIngredients", new { recipeId = newRecipeId });
        }

        private void LoadDropdowns()
        {
            var categories = _categoryService.GetAllCategories();
            CategoryOptions = categories
                .Select(c => new SelectListItem { Value = c.CategoryId.ToString(), Text = c.Name })
                .ToList();

            var difficulties = _difficultyService.GetAll();
            DifficultyOptions = difficulties
                .Select(d => new SelectListItem { Value = d.DifficultyId.ToString(), Text = d.Name })
                .ToList();
        }
    }
}