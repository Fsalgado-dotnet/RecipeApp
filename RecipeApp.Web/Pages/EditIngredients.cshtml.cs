using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RecipeApp.Models;
using RecipeApp.Services;
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

        // MUDANÇA: De long para string para aceitar o texto do datalist
        [BindProperty]
        public string IngredientName { get; set; } = string.Empty;

        [BindProperty]
        public string Quantity { get; set; } = string.Empty;

        [BindProperty]
        public string Unit { get; set; } = string.Empty;

        public void OnGet(long recipeId)
        {
            LoadData(recipeId);
        }

        public IActionResult OnPostAdd(long recipeId)
        {
            // Verificamos se o nome foi preenchido em vez do ID
            if (!string.IsNullOrWhiteSpace(IngredientName) && !string.IsNullOrEmpty(Quantity))
            {
                string fullQuantity = $"{Quantity} {Unit}".Trim();

                // Agora passamos a string IngredientName (Erro CS1503 resolvido)
                _recipeService.AddIngredientToRecipe(recipeId, IngredientName, fullQuantity);

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
            var r = _recipeService.GetById(recipeId);
            if (r != null) Recipe = r;

            Ingredients = _recipeService.GetRecipeIngredients(recipeId);
            AllIngredients = _ingredientService.GetAllIngredients();
        }
    }
}