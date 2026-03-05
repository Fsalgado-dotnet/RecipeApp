using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RecipeApp.Models;
using RecipeApp.Services;
using System.Collections.Generic;

namespace RecipeApp.Web.Pages
{
    public class RecipeDetailsModel : PageModel
    {
        private readonly RecipeService _recipeService;
        private readonly CommentService _commentService;
        private readonly RatingService _ratingService;
        private readonly FavoriteService _favoriteService;

        public RecipeDetailsModel(
            RecipeService recipeService,
            CommentService commentService,
            RatingService ratingService,
            FavoriteService favoriteService)
        {
            _recipeService = recipeService;
            _commentService = commentService;
            _ratingService = ratingService;
            _favoriteService = favoriteService;
        }

        public Recipe Recipe { get; set; }
        public List<Comment> Comments { get; set; } = new();
        public List<Ingredient> Ingredients { get; set; } = new(); // Mudado para Ingredient
        public double AverageRating { get; set; }
        public bool IsFavorite { get; set; }

        [BindProperty]
        public int SelectedRating { get; set; }

        [BindProperty]
        public string NewComment { get; set; }

        public IActionResult OnGet(long id)
        {
            // 1. Corrigido: GetById em vez de GetRecipeById
            Recipe = _recipeService.GetById(id);

            if (Recipe == null)
                return RedirectToPage("/Index");

            Comments = _commentService.GetRecipeComments(id);

            // 2. Corrigido: Usar o _recipeService diretamente para ingredientes
            Ingredients = _recipeService.GetRecipeIngredients(id);

            // 3. Corrigido: Usar a propriedade AverageRating da Recipe ou do RatingService
            AverageRating = Recipe.AverageRating;

            var user = SessionHelper.GetUser(HttpContext);
            if (user != null)
            {
                // Vamos adicionar este método ao FavoriteService abaixo
                IsFavorite = _favoriteService.CheckIfFavorite(user.UserId, id);
            }

            return Page();
        }

        public IActionResult OnPostAddComment(long id)
        {
            var user = SessionHelper.GetUser(HttpContext);
            if (user == null) return RedirectToPage("/Login");

            if (!string.IsNullOrWhiteSpace(NewComment))
            {
                _commentService.AddComment(id, user.UserId, NewComment);
                TempData["SuccessMessage"] = "Comentário adicionado!";
            }

            return RedirectToPage(new { id });
        }

        public IActionResult OnPostRate(long id)
        {
            var user = SessionHelper.GetUser(HttpContext);
            if (user == null) return RedirectToPage("/Login");

            _ratingService.SubmitRating(id, user.UserId, SelectedRating);
            TempData["SuccessMessage"] = "Obrigado pela sua avaliação!";

            return RedirectToPage(new { id });
        }

        public IActionResult OnPostToggleFavorite(long id, bool isAdding)
        {
            var user = SessionHelper.GetUser(HttpContext);
            if (user == null) return RedirectToPage("/Login");

            if (isAdding)
                _favoriteService.AddFavorite(user.UserId, id);
            else
                _favoriteService.RemoveFavorite(user.UserId, id);

            return RedirectToPage(new { id });
        }
    }
}