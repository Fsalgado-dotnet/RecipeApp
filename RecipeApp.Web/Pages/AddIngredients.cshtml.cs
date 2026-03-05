using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RecipeApp.Web.Models;
using RecipeApp.Web.Services;
using System.Collections.Generic;

namespace RecipeApp.Web.Pages
{
    public class AddIngredientsModel : PageModel
    {
        private readonly RecipeService _recipeService;
        private readonly IngredientService _ingredientService;

        public AddIngredientsModel(
            RecipeService recipeService,
            IngredientService ingredientService)
        {
            _recipeService = recipeService;
            _ingredientService = ingredientService;
        }

        public long RecipeId { get; set; }

        // Inicializamos com string.Empty para remover avisos de nulo
        public string RecipeTitle { get; set; } = string.Empty;

        public List<Ingredient> AllIngredients { get; set; } = new();
        public List<Ingredient> CurrentRecipeIngredients { get; set; } = new();

        [BindProperty]
        public long SelectedIngredientId { get; set; }

        [BindProperty]
        public string Quantity { get; set; } = string.Empty;

        // CORREÇÃO: Adicionada a propriedade Unit que faltava (Erro image_acd0be)
        [BindProperty]
        public string Unit { get; set; } = string.Empty;

        public IActionResult OnGet(long recipeId)
        {
            var user = SessionHelper.GetUser(HttpContext);
            if (user == null) return RedirectToPage("/Login");

            // Usando o método GetById que atualizámos no Service
            var recipe = _recipeService.GetById(recipeId);
            if (recipe == null) return RedirectToPage("/Index");

            RecipeId = recipeId;
            RecipeTitle = recipe.Title;

            AllIngredients = _ingredientService.GetAllIngredients();
            CurrentRecipeIngredients = _recipeService.GetRecipeIngredients(recipeId);

            return Page();
        }

        public IActionResult OnPost(long recipeId)
        {
            var user = SessionHelper.GetUser(HttpContext);
            if (user == null) return RedirectToPage("/Login");

            if (SelectedIngredientId > 0 && !string.IsNullOrEmpty(Quantity))
            {
                // Concatenamos a quantidade com a unidade se necessário, 
                // ou passamos separadamente conforme a lógica do teu Service
                string fullQuantity = string.IsNullOrEmpty(Unit) ? Quantity : $"{Quantity} {Unit}";

                _recipeService.AddIngredientToRecipe(recipeId, SelectedIngredientId, fullQuantity);

                TempData["SuccessMessage"] = "Ingrediente adicionado!";
            }

            return RedirectToPage(new { recipeId });
        }
    }
}