using RecipeApp.DAL;
using RecipeApp.Models;
using System.Collections.Generic;

namespace RecipeApp.Services
{
    public class DifficultyService
    {
        private readonly DifficultyDAL _difficultyDal;

        public DifficultyService(DifficultyDAL difficultyDal)
        {
            _difficultyDal = difficultyDal;
        }

        public List<Difficulty> GetAll()
        {
            // Lógica de Negócio:filtrar ou transformar os nomes aqui
            return _difficultyDal.GetAll();
        }
    }
}