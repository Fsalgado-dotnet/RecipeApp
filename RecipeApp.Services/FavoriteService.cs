using RecipeApp.DAL;
using RecipeApp.Models;
using System.Collections.Generic;

namespace RecipeApp.Services
{
    public class FavoriteService
    {
        private readonly RecipeDAL _recipeDal; 

        public FavoriteService(RecipeDAL recipeDal)
        {
            _recipeDal = recipeDal;
        }

        public List<Recipe> GetUserFavoriteRecipes(long userId)
        {
            // Agora usamos o método do RecipeDAL que tem o SQL completo (JOINs e COUNTs)
            return _recipeDal.GetFavoriteRecipes(userId);
        }

        public void RemoveFavorite(long userId, long recipeId)
        {
            // O RecipeDAL já tem o ToggleFavorite que serve para adicionar/remover
            _recipeDal.ToggleFavorite(userId, recipeId);
        }

        public void AddFavorite(long userId, long recipeId)
        {
            // Usamos o mesmo método, ele gere a lógica de inserir se não existir
            _recipeDal.ToggleFavorite(userId, recipeId);
        }

        public bool CheckIfFavorite(long userId, long recipeId)
        {
            // Verificamos se a receita específica está nos favoritos
            var favorites = _recipeDal.GetFavoriteRecipes(userId);
            return favorites.Exists(f => f.RecipeId == recipeId);
        }
    }
}