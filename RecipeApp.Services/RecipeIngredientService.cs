using RecipeApp.DAL;
using RecipeApp.Models;

namespace RecipeApp.Services
{
    public class RecipeIngredientService
    {
        private readonly RecipeIngredientDAL _recipeIngredientDal;

        public RecipeIngredientService(RecipeIngredientDAL recipeIngredientDal)
        {
            _recipeIngredientDal = recipeIngredientDal;
        }

        public void AddIngredientToRecipe(RecipeIngredient ri)
        {
            // REGRA DE NEGÓCIO: Validar quantidade
            if (ri.Quantity <= 0)
            {
                throw new ArgumentException("A quantidade deve ser maior que zero.");
            }

            // REGRA DE NEGÓCIO: Normalizar texto da unidade (ex: sempre minúsculas)
            ri.Unit = ri.Unit?.Trim().ToLower() ?? string.Empty;

            _recipeIngredientDal.Add(ri);
        }

        public List<RecipeIngredient> GetIngredientsByRecipe(long recipeId)
        {
            // Se a base de dados falhar, o DAL retorna lista vazia pelo try-catch
            return _recipeIngredientDal.GetByRecipe(recipeId);
        }

        public void RemoveIngredientFromRecipe(long recipeId, long ingredientId)
        {
            _recipeIngredientDal.Remove(recipeId, ingredientId);
        }
    }
}