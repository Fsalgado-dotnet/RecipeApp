using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RecipeApp.Web.DAL;
using RecipeApp.Web.Models;
using RecipeApp.Web.Services;

namespace RecipeApp.Web.Pages
{
    public class RecipeDetailsModel : PageModel
    {
        private readonly RecipeDAL _recipeDAL;
        private readonly CommentDAL _commentDAL;
        private readonly RatingDAL _ratingDAL;
        private readonly FavoriteDAL _favoriteDAL;
        private readonly RecipeIngredientDAL _recipeIngredientDAL;

        public RecipeDetailsModel(
            RecipeDAL recipeDAL,
            CommentDAL commentDAL,
            RatingDAL ratingDAL,
            FavoriteDAL favoriteDAL,
            RecipeIngredientDAL recipeIngredientDAL)
        {
            _recipeDAL = recipeDAL;
            _commentDAL = commentDAL;
            _ratingDAL = ratingDAL;
            _favoriteDAL = favoriteDAL;
            _recipeIngredientDAL = recipeIngredientDAL;
        }

        // ===============================
        // DATA
        // ===============================
        public Recipe Recipe { get; set; }
        public List<Comment> Comments { get; set; } = new();
        public List<RecipeIngredient> Ingredients { get; set; } = new();

        // ⭐ Rating
        public double AverageRating { get; set; }

        [BindProperty]
        public int SelectedRating { get; set; }

        // 💬 Comment
        [BindProperty]
        public string NewComment { get; set; }

        // ⭐ Favorite
        public bool IsFavorite { get; set; }

        // ===============================
        // GET
        // ===============================
        public IActionResult OnGet(long id)
        {
            Recipe = _recipeDAL.GetRecipeById(id);
            if (Recipe == null)
                return RedirectToPage("/Index");

            Comments = _commentDAL.GetByRecipe(id);
            Ingredients = _recipeIngredientDAL.GetByRecipe(id);
            AverageRating = _ratingDAL.GetAverageRating(id);

            var user = SessionHelper.GetUser(HttpContext);
            if (user != null)
            {
                IsFavorite = _favoriteDAL.IsFavorite(user.UserId, id);
            }

            return Page();
        }

        // ===============================
        // COMMENTS
        // ===============================
        public IActionResult OnPostAddComment(long id)
        {
            if (!SessionHelper.IsLoggedIn(HttpContext))
                return RedirectToPage("/Login");

            var user = SessionHelper.GetUser(HttpContext);

            _commentDAL.Create(new Comment
            {
                RecipeId = id,
                UserId = user.UserId,
                Text = NewComment,
                CreatedAt = DateTime.Now
            });

            return RedirectToPage(new { id });
        }

        // ===============================
        // RATING
        // ===============================
        public IActionResult OnPostRate(long id)
        {
            if (!SessionHelper.IsLoggedIn(HttpContext))
                return RedirectToPage("/Login");

            var user = SessionHelper.GetUser(HttpContext);
            _ratingDAL.SaveRating(id, user.UserId, SelectedRating);

            return RedirectToPage(new { id });
        }

        // ===============================
        // FAVORITES
        // ===============================
        public IActionResult OnPostAddFavorite(long id)
        {
            if (!SessionHelper.IsLoggedIn(HttpContext))
                return RedirectToPage("/Login");

            var user = SessionHelper.GetUser(HttpContext);
            _favoriteDAL.Add(user.UserId, id);

            return RedirectToPage(new { id });
        }

        public IActionResult OnPostRemoveFavorite(long id)
        {
            if (!SessionHelper.IsLoggedIn(HttpContext))
                return RedirectToPage("/Login");

            var user = SessionHelper.GetUser(HttpContext);
            _favoriteDAL.Remove(user.UserId, id);

            return RedirectToPage(new { id });
        }
    }
}
