using RecipeApp.DAL;
using RecipeApp.Models;
using System.Collections.Generic;

namespace RecipeApp.Services
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
            return _recipeDal.GetApprovedRecipes(userId, search, cat, diff, sort);
        }

        // 2. Para Detalhes e Edição
        public Recipe? GetById(long id)
        {
            return _recipeDal.GetRecipeById(id);
        }

        // 3. Para a página MyRecipes
        public List<Recipe> GetUserRecipes(long userId)
        {
            return _recipeDal.GetRecipesByUserId(userId);
        }

        // 4. Criar Receitas
        public int CreateRecipe(Recipe recipe, long userId)
        {
            recipe.CreatedByUserId = userId;
            recipe.IsApproved = false;
            return _recipeDal.CreateRecipe(recipe);
        }

        // 5. Atualizar Receitas
        public void UpdateRecipe(Recipe recipe)
        {
            _recipeDal.UpdateRecipe(recipe);
        }

        // 6. Lógica de Remoção (Soft Delete)
        public bool DeleteRecipe(long recipeId, long userId)
        {
            return _recipeDal.DeleteRecipe(recipeId, userId);
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

        // 7. Favoritos
        public void ToggleFavorite(long userId, long recipeId)
        {
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

        // 9. Ingredientes (LÓGICA ACTUALIZADA)
        public List<Ingredient> GetRecipeIngredients(long recipeId)
        {
            return _recipeDal.GetIngredientsByRecipeId(recipeId);
        }

        /// <summary>
        /// Adiciona um ingrediente à receita. 
        /// Se o nome for fornecido e não existir na DB, ele é criado automaticamente.
        /// </summary>
        public void AddIngredientToRecipe(long recipeId, string ingredientName, string quantity)
        {
            if (string.IsNullOrWhiteSpace(ingredientName)) return;

            // 1. Vai à DAL buscar o ID existente ou criar um novo pelo nome
            long ingredientId = _recipeDal.GetOrCreateIngredient(ingredientName);

            // 2. Faz a ligação na tabela RecipeIngredient
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