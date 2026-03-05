using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RecipeApp.Models;
using RecipeApp.Services;
using System.Collections.Generic;

namespace RecipeApp.Web.Pages
{
    public class MyFavoritesModel : PageModel
    {
        private readonly FavoriteService _favoriteService;

        public MyFavoritesModel(FavoriteService favoriteService)
        {
            _favoriteService = favoriteService;
        }

        public List<Recipe> Recipes { get; set; } = new();

        public IActionResult OnGet()
        {
            // 1. Verificação de Segurança
            if (!SessionHelper.IsLoggedIn(HttpContext))
                return RedirectToPage("/Login");

            var user = SessionHelper.GetUser(HttpContext);

            // 2. O Serviço agora gere a junção entre a tabela de favoritos e a de receitas
            Recipes = _favoriteService.GetUserFavoriteRecipes(user.UserId);

            return Page();
        }

        public IActionResult OnPostRemove(long id)
        {
            if (!SessionHelper.IsLoggedIn(HttpContext))
                return RedirectToPage("/Login");

            var user = SessionHelper.GetUser(HttpContext);

            // 3. Remoção via Serviço
            _favoriteService.RemoveFavorite(user.UserId, id);

            TempData["SuccessMessage"] = "Receita removida dos teus favoritos.";

            return RedirectToPage();
        }
    }
}