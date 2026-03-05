using RecipeApp.Web.DAL;
using RecipeApp.Web.Models;
using System.Collections.Generic;

namespace RecipeApp.Web.Services
{
    public class IngredientService
    {
        private readonly IngredientDAL _ingredientDal;

        public IngredientService(IngredientDAL ingredientDal)
        {
            _ingredientDal = ingredientDal;
        }

        public List<Ingredient> GetAllIngredients()
        {
            // Lógica de Negócio:  filtrar ingredientes inativos 
            // ou ordenar de forma diferente se necessário.
            return _ingredientDal.GetAll();
        }
    }
}