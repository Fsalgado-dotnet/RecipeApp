using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using RecipeApp.Web.Models;
using RecipeApp.Web.Services;

namespace RecipeApp.Web.Pages
{
    public class EditRecipeModel : PageModel
    {
        private readonly RecipeService _recipeService;
        private readonly CategoryService _categoryService;
        private readonly DifficultyService _difficultyService;

        public EditRecipeModel(
            RecipeService recipeService,
            CategoryService categoryService,
            DifficultyService difficultyService)
        {
            _recipeService = recipeService;
            _categoryService = categoryService;
            _difficultyService = difficultyService;
        }

        [BindProperty]
        public Recipe Recipe { get; set; }

        public List<SelectListItem> Categories { get; set; }
        public List<SelectListItem> Difficulties { get; set; }

        public IActionResult OnGet(long id)
        {
            // 1. Verificação de Sessão
            if (!SessionHelper.IsLoggedIn(HttpContext))
                return RedirectToPage("/Login");

            // 2. Obter a receita via Serviço
            Recipe = _recipeService.GetById(id);

            if (Recipe == null)
                return RedirectToPage("/Index");

            // 3. Verificação de Permissões (Dono ou Admin)
            var user = SessionHelper.GetUser(HttpContext);
            if (!SessionHelper.IsAdmin(HttpContext) && Recipe.CreatedByUserId != user.UserId)
            {
                return RedirectToPage("/Index");
            }

            LoadDropdowns();
            return Page();
        }

        public IActionResult OnPost()
        {
            if (!SessionHelper.IsLoggedIn(HttpContext))
                return RedirectToPage("/Login");

            // No Update, o serviço pode validar se os dados estão corretos antes de salvar
            _recipeService.UpdateRecipe(Recipe);

            TempData["SuccessMessage"] = "Receita atualizada com sucesso!";
            return RedirectToPage("/MyRecipes");
        }

        private void LoadDropdowns()
        {
            Categories = _categoryService.GetAllCategories()
                .Select(c => new SelectListItem { Value = c.CategoryId.ToString(), Text = c.Name })
                .ToList();

            Difficulties = _difficultyService.GetAll()
                .Select(d => new SelectListItem { Value = d.DifficultyId.ToString(), Text = d.Name })
                .ToList();
        }
    }
}