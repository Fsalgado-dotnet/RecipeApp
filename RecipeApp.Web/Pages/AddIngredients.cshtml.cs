using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RecipeApp.Models;
using RecipeApp.Services;
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

        // Propriedade para identificar a receita
        public long RecipeId { get; set; }

        public string RecipeTitle { get; set; } = string.Empty;

        public List<Ingredient> AllIngredients { get; set; } = new();
        public List<Ingredient> CurrentRecipeIngredients { get; set; } = new();

        // MUDANÇA CHAVE: Agora recebemos o NOME (string) do datalist, não o ID para o utlizador ter opcao de escrever o nome do ingrediente.
        [BindProperty]
        public string IngredientName { get; set; } = string.Empty;

        [BindProperty]
        public string Quantity { get; set; } = string.Empty;

        [BindProperty]
        public string Unit { get; set; } = string.Empty;

        public IActionResult OnGet(long recipeId)
        {
            var user = SessionHelper.GetUser(HttpContext);
            if (user == null) return RedirectToPage("/Login");

            var recipe = _recipeService.GetById(recipeId);
            if (recipe == null) return RedirectToPage("/Index");

            RecipeId = recipeId;
            RecipeTitle = recipe.Title;

            LoadData(recipeId);

            return Page();
        }

        public IActionResult OnPost(long recipeId)
        {
            var user = SessionHelper.GetUser(HttpContext);
            if (user == null) return RedirectToPage("/Login");

            // Validamos se o nome do ingrediente foi preenchido
            if (!string.IsNullOrWhiteSpace(IngredientName) && !string.IsNullOrEmpty(Quantity))
            {
                // Concatenamos a quantidade com a unidade
                string fullQuantity = string.IsNullOrEmpty(Unit) ? Quantity : $"{Quantity} {Unit}";

                // Agora enviamos IngredientName (string) para o Service
                // Isso resolve o erro CS1503 (long para string)
                _recipeService.AddIngredientToRecipe(recipeId, IngredientName, fullQuantity);

                TempData["SuccessMessage"] = "Ingrediente adicionado!";
            }

            return RedirectToPage(new { recipeId });
        }

        private void LoadData(long recipeId)
        {
            AllIngredients = _ingredientService.GetAllIngredients();
            CurrentRecipeIngredients = _recipeService.GetRecipeIngredients(recipeId);
        }
    }
}