using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RecipeApp.Web.DAL;
using RecipeApp.Web.Models;
using RecipeApp.Web.Services;

namespace RecipeApp.Web.Pages
{
    public class EditIngredientsModel : PageModel
    {
        private readonly RecipeDAL _recipeDAL;
        private readonly IngredientDAL _ingredientDAL;
        private readonly RecipeIngredientDAL _recipeIngredientDAL;

        public EditIngredientsModel(
            RecipeDAL recipeDAL,
            IngredientDAL ingredientDAL,
            RecipeIngredientDAL recipeIngredientDAL)
        {
            _recipeDAL = recipeDAL;
            _ingredientDAL = ingredientDAL;
            _recipeIngredientDAL = recipeIngredientDAL;
        }

        public Recipe Recipe { get; set; }
        public List<RecipeIngredient> Ingredients { get; set; } = new();
        public List<Ingredient> AllIngredients { get; set; } = new();

        [BindProperty]
        public long IngredientId { get; set; }

        [BindProperty]
        public double Quantity { get; set; }

        [BindProperty]
        public string Unit { get; set; }

        // ===============================
        // GET
        // ===============================
        public IActionResult OnGet(long recipeId)
        {
            if (!SessionHelper.IsLoggedIn(HttpContext))
                return RedirectToPage("/Login");

            Recipe = _recipeDAL.GetRecipeById(recipeId);
            if (Recipe == null)
                return RedirectToPage("/Index");

            var user = SessionHelper.GetUser(HttpContext);

            if (!SessionHelper.IsAdmin(HttpContext) &&
                Recipe.CreatedByUserId != user.UserId)
            {
                return RedirectToPage("/Index");
            }

            Ingredients = _recipeIngredientDAL.GetByRecipe(recipeId);
            AllIngredients = _ingredientDAL.GetAll();

            return Page();
        }

        // ===============================
        // ADD INGREDIENT
        // ===============================
        public IActionResult OnPostAdd(long recipeId)
        {
            if (!SessionHelper.IsLoggedIn(HttpContext))
                return RedirectToPage("/Login");

            _recipeIngredientDAL.Add(new RecipeIngredient
            {
                RecipeId = recipeId,
                IngredientId = IngredientId,
                Quantity = Quantity,
                Unit = Unit
            });

            return RedirectToPage(new { recipeId });
        }

        // ===============================
        // REMOVE INGREDIENT
        // ===============================
        public IActionResult OnPostRemove(long recipeId, long ingredientId)
        {
            if (!SessionHelper.IsLoggedIn(HttpContext))
                return RedirectToPage("/Login");

            _recipeIngredientDAL.Remove(recipeId, ingredientId);

            return RedirectToPage(new { recipeId });
        }
    }
}

