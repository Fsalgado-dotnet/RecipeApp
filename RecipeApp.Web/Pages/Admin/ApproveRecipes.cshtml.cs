using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RecipeApp.Web.DAL;
using RecipeApp.Web.Models;
using RecipeApp.Web.Services;

namespace RecipeApp.Web.Pages.Admin
{
    public class ApproveRecipesModel : PageModel
    {
        private readonly RecipeDAL _recipeDAL;

        public ApproveRecipesModel(RecipeDAL recipeDAL)
        {
            _recipeDAL = recipeDAL;
        }

        public List<Recipe> PendingRecipes { get; set; }

        public IActionResult OnGet()
        {
            if (!SessionHelper.IsAdmin(HttpContext))
                return RedirectToPage("/Index");

            PendingRecipes = _recipeDAL.GetPendingRecipes();
            return Page();
        }

        public IActionResult OnPostApprove(long recipeId)
        {
            if (!SessionHelper.IsAdmin(HttpContext))
                return RedirectToPage("/Index");

            _recipeDAL.ApproveRecipe(recipeId);
            return RedirectToPage();
        }
    }
}
