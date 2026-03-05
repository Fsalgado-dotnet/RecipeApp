using RecipeApp.Web.DAL;
using RecipeApp.Web.Models;
using System.Collections.Generic;

namespace RecipeApp.Web.Services
{
    public class RecipeService
    {
        private readonly RecipeDAL _recipeDal;

        public RecipeService(RecipeDAL recipeDal)
        {
            _recipeDal = recipeDal;
        }

        // 1. Para o Index (Home)
        public List<Recipe> GetHomeRecipes(long? userId, string? search, long? cat, long? diff, string sort)
        {
            return _recipeDal.GetApprovedRecipes(userId, search, cat, diff);
        }

        // 2. Para Detalhes e Edição (Resolve erros em RecipeDetails e EditRecipe)
        public Recipe? GetById(long id)
        {
            return _recipeDal.GetRecipeById(id);
        }

        // 3. Para a página MyRecipes
        public List<Recipe> GetUserRecipes(long userId)
        {
            return _recipeDal.GetRecipesByUserId(userId);
        }

        // 4. Para Criar Receitas
        public void CreateRecipe(Recipe recipe, long userId)
        {
            recipe.CreatedByUserId = userId;
            recipe.IsApproved = false;
            _recipeDal.CreateRecipe(recipe);
        }

        // 5. Para Atualizar Receitas
        public void UpdateRecipe(Recipe recipe)
        {
            _recipeDal.UpdateRecipe(recipe);
        }

        // 6. Lógica de Remoção (Soft Delete)
        public void DeleteRecipe(long recipeId, long userId)
        {
            _recipeDal.DeleteRecipe(recipeId, userId);
        }

        public bool SoftDelete(long recipeId, long userId, bool isAdmin)
        {
            if (isAdmin)
            {
                var recipe = _recipeDal.GetRecipeById(recipeId);
                if (recipe != null) return _recipeDal.DeleteRecipe(recipeId, recipe.CreatedByUserId);
            }
            return _recipeDal.DeleteRecipe(recipeId, userId);
        }

        // 7. Favoritos (CORREÇÃO DO ERRO CS0103)
        public void ToggleFavorite(long userId, long recipeId)
        {
            // Alterado de _recipeDAL para _recipeDal (minúsculas)
            _recipeDal.ToggleFavorite(userId, recipeId);
        }

        // 8. Admin e Moderação
        public List<Recipe> GetPendingRecipes()
        {
            return _recipeDal.GetPendingRecipes();
        }

        public void ApproveRecipe(long recipeId)
        {
            _recipeDal.ApproveRecipe(recipeId);
        }

        // 9. Ingredientes
        public List<Ingredient> GetRecipeIngredients(long recipeId)
        {
            return _recipeDal.GetIngredientsByRecipeId(recipeId);
        }

        public void AddIngredientToRecipe(long recipeId, long ingredientId, string quantity)
        {
            _recipeDal.AddIngredientToRecipe(recipeId, ingredientId, quantity);
        }

        public void RemoveIngredientFromRecipe(long recipeId, long ingredientId)
        {
            _recipeDal.DeleteIngredientFromRecipe(recipeId, ingredientId);
        }

        // 10. Auxiliares de Permissão
        public bool UserCanManageRecipe(long recipeId, long userId, bool isAdmin)
        {
            if (isAdmin) return true;
            var recipe = _recipeDal.GetRecipeById(recipeId);
            return recipe != null && recipe.CreatedByUserId == userId;
        }
    }
}