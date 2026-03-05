using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RecipeApp.Web.Models;
using RecipeApp.Web.Services;
using System.Collections.Generic;

namespace RecipeApp.Web.Pages
{
    public class MyRecipesModel : PageModel
    {
        private readonly RecipeService _recipeService;

        public MyRecipesModel(RecipeService recipeService)
        {
            _recipeService = recipeService;
        }

        public List<Recipe> Recipes { get; set; } = new();

        public IActionResult OnGet()
        {
            // 1. Verificação de Segurança (SessionHelper)
            if (!SessionHelper.IsLoggedIn(HttpContext))
                return RedirectToPage("/Login");

            // 2. Obter Utilizador da Sessão
            var user = SessionHelper.GetUser(HttpContext);

            // 3. Obter Receitas via Serviço (Abstração do DAL)
            // Nota: O serviço tratará de chamar o _recipeDAL.GetByUser internamente
            Recipes = _recipeService.GetUserRecipes(user.UserId);

            return Page();
        }

        // Handler opcional para eliminar rapidamente a partir da lista
        public IActionResult OnPostDelete(long id)
        {
            var user = SessionHelper.GetUser(HttpContext);
            if (user == null) return RedirectToPage("/Login");

            _recipeService.DeleteRecipe(id, user.UserId);
            TempData["SuccessMessage"] = "Receita eliminada com sucesso.";

            return RedirectToPage();
        }
    }
}