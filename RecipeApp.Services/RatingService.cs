using RecipeApp.DAL;
using System;

namespace RecipeApp.Services
{
    public class RatingService
    {
        private readonly RatingDAL _ratingDal;

        public RatingService(RatingDAL ratingDal)
        {
            _ratingDal = ratingDal;
        }

        
        public void SubmitRating(long recipeId, long userId, int value)
        {
            if (value < 1 || value > 5)
                throw new ArgumentException("A avaliação deve ser entre 1 e 5 estrelas.");

            _ratingDal.SaveRating(recipeId, userId, value);
        }

        
        public double GetAverageRating(long recipeId)
        {
            return _ratingDal.GetAverageRating(recipeId);
        }
    }
}