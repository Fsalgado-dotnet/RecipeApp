using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RecipeApp.Models;
using RecipeApp.Services;
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

            // Ajuste defensivo: se a sessão expirou ou user é nulo
            if (user == null) return RedirectToPage("/Login");

            // 3. Obter Receitas via Serviço
            // O objeto 'recipe' que isto devolve já deve conter a propriedade RejectionReason
            Recipes = _recipeService.GetUserRecipes(user.UserId);

            return Page();
        }

        // Handler para eliminar a receita
        public IActionResult OnPostDelete(long id)
        {
            // 1. Validar se o utilizador está logado
            var user = SessionHelper.GetUser(HttpContext);
            if (user == null) return RedirectToPage("/Login");

            // 2. Chamar o serviço de eliminação
            bool success = _recipeService.DeleteRecipe(id, user.UserId);

            if (success)
            {
                TempData["SuccessMessage"] = "Receita removida definitivamente.";
            }
            else
            {
                TempData["ErrorMessage"] = "Não foi possível eliminar a receita ou não tens permissão.";
            }

            return RedirectToPage();
        }
    }
}