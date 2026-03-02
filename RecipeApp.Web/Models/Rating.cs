namespace RecipeApp.Web.Models
{
    public class Rating
    {
        public int RatingId { get; set; }
        public long RecipeId { get; set; }
        public long UserId { get; set; }
        public int Value { get; set; } // 1–5
        public DateTime CreatedAt { get; set; }
    }
}
