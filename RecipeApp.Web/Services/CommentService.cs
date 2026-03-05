using RecipeApp.Web.DAL;
using RecipeApp.Web.Models;
using System.Collections.Generic;

namespace RecipeApp.Web.Services
{
    public class CommentService
    {
        private readonly CommentDAL _commentDal;

        public CommentService(CommentDAL commentDal)
        {
            _commentDal = commentDal;
        }

        public void AddComment(long recipeId, long userId, string text)
        {
            // LÓGICA DE NEGÓCIO: Validar se o texto não é nulo ou apenas espaços
            if (string.IsNullOrWhiteSpace(text))
            {
                throw new ArgumentException("O comentário não pode estar vazio.");
            }

            // LÓGICA DE NEGÓCIO: Limitar tamanho ( 500 caracteres)
            if (text.Length > 500)
            {
                text = text.Substring(0, 500);
            }

            var newComment = new Comment
            {
                RecipeId = recipeId,
                UserId = userId,
                Text = text.Trim(),
                CreatedAt = DateTime.Now // Centralizamos a data aqui
            };

            _commentDal.Create(newComment);
        }

        public List<Comment> GetRecipeComments(long recipeId)
        {
            return _commentDal.GetByRecipe(recipeId);
        }
    }
}