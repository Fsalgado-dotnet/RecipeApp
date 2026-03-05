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

            // 3. Obter Receitas via Serviço
            // Agora o DAL filtrará corretamente apenas as receitas ativas (IsApproved >= 0)
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
            // Com a nova RecipeDAL, isto agora limpa ingredientes, favoritos e a receita da DB
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