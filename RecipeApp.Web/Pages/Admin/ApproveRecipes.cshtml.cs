using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RecipeApp.Web.Models;
using RecipeApp.Web.Services; // Importante, deu-me erro umas vezes por nao chamar a SessionHelper aqui

namespace RecipeApp.Web.Pages.Admin
{
    public class ApproveRecipesModel : PageModel
    {
        private readonly RecipeService _recipeService;

        // Injetamos o Service aqui
        public ApproveRecipesModel(RecipeService recipeService)
        {
            _recipeService = recipeService;
        }

        public List<Recipe> PendingRecipes { get; set; } = new();

        public IActionResult OnGet()
        {
            if (!SessionHelper.IsAdmin(HttpContext))
                return RedirectToPage("/Index");

            // Usamos o Service agora
            PendingRecipes = _recipeService.GetPendingRecipes();
            return Page();
        }

        public IActionResult OnPostApprove(long recipeId)
        {
            if (!SessionHelper.IsAdmin(HttpContext))
                return RedirectToPage("/Index");

            _recipeService.ApproveRecipe(recipeId);
            TempData["SuccessMessage"] = "Receita aprovada com sucesso!";
            return RedirectToPage();
        }
    }
}