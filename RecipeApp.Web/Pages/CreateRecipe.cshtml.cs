using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RecipeApp.Web.DAL;
using RecipeApp.Web.Models;
using RecipeApp.Web.Services;

namespace RecipeApp.Web.Pages
{
    public class CreateRecipeModel : PageModel
    {
        private readonly RecipeDAL _recipeDAL;
        private readonly CategoryDAL _categoryDAL;
        private readonly DifficultyDAL _difficultyDAL;

        public CreateRecipeModel(
            RecipeDAL recipeDAL,
            CategoryDAL categoryDAL,
            DifficultyDAL difficultyDAL)
        {
            _recipeDAL = recipeDAL;
            _categoryDAL = categoryDAL;
            _difficultyDAL = difficultyDAL;
        }

        [BindProperty]
        public Recipe Recipe { get; set; }

        public List<Category> Categories { get; set; } = new();
        public List<Difficulty> Difficulties { get; set; } = new();

        public IActionResult OnGet()
        {
            if (!SessionHelper.IsLoggedIn(HttpContext))
                return RedirectToPage("/Login");

            Categories = _categoryDAL.GetAll();
            Difficulties = _difficultyDAL.GetAll();

            return Page();
        }

        public IActionResult OnPost()
        {
            if (!SessionHelper.IsLoggedIn(HttpContext))
                return RedirectToPage("/Login");

            var user = SessionHelper.GetUser(HttpContext);

            Recipe.CreatedByUserId = user.UserId;
            Recipe.IsApproved = false;
            Recipe.CreatedAt = DateTime.Now;

            _recipeDAL.CreateRecipe(Recipe);

            return RedirectToPage("/Index");
        }
    }
}
