using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RecipeApp.Web.Models;
using RecipeApp.Web.Services;
using System.Collections.Generic;

namespace RecipeApp.Web.Pages
{
    public class EditIngredientsModel : PageModel
    {
        private readonly RecipeService _recipeService;
        private readonly IngredientService _ingredientService;

        public EditIngredientsModel(RecipeService recipeService, IngredientService ingredientService)
        {
            _recipeService = recipeService;
            _ingredientService = ingredientService;
        }

        public Recipe Recipe { get; set; } = new();
        public List<Ingredient> Ingredients { get; set; } = new();
        public List<Ingredient> AllIngredients { get; set; } = new();

        [BindProperty]
        public long SelectedIngredientId { get; set; }

        [BindProperty]
        public string Quantity { get; set; }

        [BindProperty]
        public string Unit { get; set; }

        public void OnGet(long recipeId)
        {
            LoadData(recipeId);
        }

        public IActionResult OnPostAdd(long recipeId)
        {
            if (SelectedIngredientId > 0 && !string.IsNullOrEmpty(Quantity))
            {
                string fullQuantity = $"{Quantity} {Unit}".Trim();
                _recipeService.AddIngredientToRecipe(recipeId, SelectedIngredientId, fullQuantity);
                TempData["SuccessMessage"] = "Ingrediente adicionado!";
            }
            return RedirectToPage(new { recipeId });
        }

        public IActionResult OnPostRemove(long recipeId, long ingredientId)
        {
            _recipeService.RemoveIngredientFromRecipe(recipeId, ingredientId);
            TempData["SuccessMessage"] = "Ingrediente removido!";
            return RedirectToPage(new { recipeId });
        }

        private void LoadData(long recipeId)
        {
            
            Recipe = _recipeService.GetById(recipeId);

          
            Ingredients = _recipeService.GetRecipeIngredients(recipeId);


            AllIngredients = _ingredientService.GetAllIngredients();
        }
    }
}