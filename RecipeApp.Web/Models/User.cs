namespace RecipeApp.Web.Models
{
    public class User
    {
        public long UserId { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public bool IsAdmin { get; set; }
        public bool IsLocked { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
