using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RecipeApp.Web.DAL;
using RecipeApp.Web.Models;
using RecipeApp.Web.Services;

namespace RecipeApp.Web.Pages
{
    public class MyRecipesModel : PageModel
    {
        private readonly RecipeDAL _recipeDAL;

        public MyRecipesModel(RecipeDAL recipeDAL)
        {
            _recipeDAL = recipeDAL;
        }

        public List<Recipe> Recipes { get; set; } = new();

        public IActionResult OnGet()
        {
            if (!SessionHelper.IsLoggedIn(HttpContext))
                return RedirectToPage("/Login");

            var user = SessionHelper.GetUser(HttpContext);
            Recipes = _recipeDAL.GetByUser(user.UserId);

            return Page();
        }
    }
}
