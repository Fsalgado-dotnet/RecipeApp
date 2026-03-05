using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using RecipeApp.Web.Models;
using RecipeApp.Web.Services;
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
            if (!SessionHelper.IsLoggedIn(HttpContext))
                return RedirectToPage("/Login");

            LoadDropdowns();
            return Page();
        }

        public IActionResult OnPost()
        {
            var user = SessionHelper.GetUser(HttpContext);
            if (user == null) return RedirectToPage("/Login");

            
            _recipeService.CreateRecipe(Recipe, user.UserId);

            TempData["SuccessMessage"] = "Receita enviada com sucesso! Aguarde a aprovação.";

            // Redirecionamos para o Index ou para as receitas
            return RedirectToPage("/Index");
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