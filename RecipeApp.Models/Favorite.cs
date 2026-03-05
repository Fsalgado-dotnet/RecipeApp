namespace RecipeApp.Models
{
    public class Favorite
    {
        public long UserId { get; set; }
        public long RecipeId { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}

