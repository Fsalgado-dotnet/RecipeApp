namespace RecipeApp.Models
{
    public class Comment
    {
        public long CommentId { get; set; }

        public long RecipeId { get; set; }
        public long UserId { get; set; }

        public string Text { get; set; } = string.Empty;

        public string UserName { get; set; } = string.Empty; 

        public DateTime CreatedAt { get; set; }
    }
}
