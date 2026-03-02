using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RecipeApp.Web.DAL;
using RecipeApp.Web.Models;
using RecipeApp.Web.Services;

namespace RecipeApp.Web.Pages
{
    public class MyFavoritesModel : PageModel
    {
        private readonly FavoriteDAL _favoriteDAL;

        public MyFavoritesModel(FavoriteDAL favoriteDAL)
        {
            _favoriteDAL = favoriteDAL;
        }

        public List<Recipe> Recipes { get; set; } = new();

        public IActionResult OnGet()
        {
            if (!SessionHelper.IsLoggedIn(HttpContext))
                return RedirectToPage("/Login");

            var user = SessionHelper.GetUser(HttpContext);

            // Busca a lista de receitas favoritas do utilizador logado
            Recipes = _favoriteDAL.GetUserFavorites(user.UserId);

            return Page();
        }

        public IActionResult OnPostRemove(long id)
        {
            if (!SessionHelper.IsLoggedIn(HttpContext))
                return RedirectToPage("/Login");

            var user = SessionHelper.GetUser(HttpContext);

          
            _favoriteDAL.Remove(user.UserId, id);

            return RedirectToPage();
        }
    }
}