using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RecipeApp.Web.DAL;
using RecipeApp.Web.Models;
using RecipeApp.Web.Services;
using System.Collections.Generic;

namespace RecipeApp.Web.Pages
{
    public class IndexModel : PageModel
    {
        private readonly RecipeDAL _recipeDAL;
        private readonly RatingDAL _ratingDAL;
        private readonly FavoriteDAL _favoriteDAL;
        private readonly CategoryDAL _categoryDAL; 
        private readonly DifficultyDAL _difficultyDAL; 

        public IndexModel(
            RecipeDAL recipeDAL,
            RatingDAL ratingDAL,
            FavoriteDAL favoriteDAL,
            CategoryDAL categoryDAL,
            DifficultyDAL difficultyDAL)
        {
            _recipeDAL = recipeDAL;
            _ratingDAL = ratingDAL;
            _favoriteDAL = favoriteDAL;
            _categoryDAL = categoryDAL;
            _difficultyDAL = difficultyDAL;
        }

        public List<Recipe> Recipes { get; set; } = new();
        public List<Category> Categories { get; set; } = new(); // 🔹 Para o filtro
        public List<Difficulty> Difficulties { get; set; } = new(); // 🔹 Para o filtro

        // 🔹 OnGet atualizado para receber os novos filtros
        public void OnGet(string searchTerm, long? categoryId, long? difficultyId)
        {
            var user = SessionHelper.GetUser(HttpContext);
            long? userId = user?.UserId;

            // 1️⃣ Carregar as listas para popular os Selects (Dropdowns) no HTML
            Categories = _categoryDAL.GetAll();
            Difficulties = _difficultyDAL.GetAll();

            // 2️⃣ Buscar receitas filtradas (Passando todos os parâmetros para o DAL)
            Recipes = _recipeDAL.GetApprovedRecipes(userId, searchTerm, categoryId, difficultyId);

            // 3️⃣ Manter a tua lógica original de Ratings e Favoritos
            foreach (var recipe in Recipes)
            {
                // ⭐ Rating médio
                recipe.AverageRating = _ratingDAL.GetAverageRating(recipe.RecipeId);

                // ❤️ Favorito (se estiver logado)
                if (user != null)
                {
                    recipe.IsFavorite = _favoriteDAL.IsFavorite(user.UserId, recipe.RecipeId);
                }
            }
        }

        // ============================================================
        // 🔹 MÉTODO PARA ELIMINAR A RECEITA
        // ============================================================
        public IActionResult OnPostDelete(long id)
        {
            // 1️⃣ Verificar se o utilizador está logado
            var user = SessionHelper.GetUser(HttpContext);
            if (user == null)
            {
                return RedirectToPage("/Login");
            }

            // 2️⃣ Chamar o DAL para fazer o "Soft Delete" (IsApproved = 0)
            // Passando ID da receita e o ID do utilizador por segurança
            _recipeDAL.DeleteRecipe(id, user.UserId);

            // 3️⃣ Recarregar a página para atualizar a lista
            return RedirectToPage();
        }
    }
}