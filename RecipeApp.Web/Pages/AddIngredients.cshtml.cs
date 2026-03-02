using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RecipeApp.Web.DAL;
using RecipeApp.Web.Models;
using RecipeApp.Web.Services;

namespace RecipeApp.Web.Pages
{
    public class AddIngredientsModel : PageModel
    {
        private readonly IngredientDAL _ingredientDAL;
        private readonly RecipeIngredientDAL _recipeIngredientDAL;

        public AddIngredientsModel(
            IngredientDAL ingredientDAL,
            RecipeIngredientDAL recipeIngredientDAL)
        {
            _ingredientDAL = ingredientDAL;
            _recipeIngredientDAL = recipeIngredientDAL;
        }

        public long RecipeId { get; set; }
        public List<Ingredient> Ingredients { get; set; } = new();
        public List<RecipeIngredient> RecipeIngredients { get; set; } = new();

        [BindProperty]
        public long IngredientId { get; set; }

        [BindProperty]
        public double Quantity { get; set; }

        [BindProperty]
        public string Unit { get; set; }

        public IActionResult OnGet(long recipeId)
        {
            if (!SessionHelper.IsLoggedIn(HttpContext))
                return RedirectToPage("/Login");

            RecipeId = recipeId;
            Ingredients = _ingredientDAL.GetAll();
            RecipeIngredients = _recipeIngredientDAL.GetByRecipe(recipeId);

            return Page();
        }

        public IActionResult OnPost(long recipeId)
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
    }
}

