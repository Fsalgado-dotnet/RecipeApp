namespace RecipeApp.Web.Models
{
    public class Recipe
    {
        // 🔑 PK
        public long RecipeId { get; set; }

        // 📄 Dados principais
        public string Title { get; set; } = string.Empty;
        public string PreparationMethod { get; set; } = string.Empty;
        public int PreparationTime { get; set; }
        // 🔗 FK
        public int CategoryId { get; set; }
        public int DifficultyId { get; set; }
        public long CreatedByUserId { get; set; }

        // 🧠 JOIN (para mostrar no UI)
        public string CategoryName { get; set; } = string.Empty;
        public string DifficultyName { get; set; } = string.Empty;
        public string AuthorName { get; set; } = string.Empty;

        // ⭐ Rating (calculado)
        public double AverageRating { get; set; }

        // ❤️ Favoritos
        public bool IsFavorite { get; set; }

        // 🔐 Estado
        public bool IsApproved { get; set; }

        // 📅 Datas
        public DateTime CreatedAt { get; set; }
    }
}
