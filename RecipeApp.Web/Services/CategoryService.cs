using RecipeApp.Web.DAL;
using RecipeApp.Web.Models;
using System.Collections.Generic;

namespace RecipeApp.Web.Services
{
    public class CategoryService
    {
        private readonly CategoryDAL _categoryDal;

        public CategoryService(CategoryDAL categoryDal)
        {
            _categoryDal = categoryDal;
        }

        public List<Category> GetAllCategories()
        {
            // Lógica de Negócio: Se precisar de esconder categorias 
            // sem receitas associadas, a lógica fica aqui.
            return _categoryDal.GetAll();
        }
    }
}