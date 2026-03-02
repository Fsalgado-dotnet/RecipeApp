using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using RecipeApp.Web.DAL;
using RecipeApp.Web.Models;
using RecipeApp.Web.Services;

namespace RecipeApp.Web.Pages
{
    public class EditRecipeModel : PageModel
    {
        private readonly RecipeDAL _recipeDAL;
        private readonly CategoryDAL _categoryDAL;
        private readonly DifficultyDAL _difficultyDAL;

        public EditRecipeModel(
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

        public List<SelectListItem> Categories { get; set; }
        public List<SelectListItem> Difficulties { get; set; }

        // ===============================
        // GET
        // ===============================
        public IActionResult OnGet(long id)
        {
            if (!SessionHelper.IsLoggedIn(HttpContext))
                return RedirectToPage("/Login");

            Recipe = _recipeDAL.GetRecipeById(id);
            if (Recipe == null)
                return RedirectToPage("/Index");

            var user = SessionHelper.GetUser(HttpContext);

            if (!SessionHelper.IsAdmin(HttpContext) &&
                Recipe.CreatedByUserId != user.UserId)
            {
                return RedirectToPage("/Index");
            }

            LoadDropdowns();
            return Page();
        }

        // ===============================
        // POST
        // ===============================
        public IActionResult OnPost()
        {
            if (!SessionHelper.IsLoggedIn(HttpContext))
                return RedirectToPage("/Login");

            _recipeDAL.Update(Recipe);
            return RedirectToPage("/MyRecipes");
        }

        private void LoadDropdowns()
        {
            Categories = _categoryDAL.GetAll()
                .Select(c => new SelectListItem
                {
                    Value = c.CategoryId.ToString(),
                    Text = c.Name
                }).ToList();

            Difficulties = _difficultyDAL.GetAll()
                .Select(d => new SelectListItem
                {
                    Value = d.DifficultyId.ToString(),
                    Text = d.Name
                }).ToList();
        }
    }
}

