using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System; // Adicionado para o DateTime

namespace RecipeApp.Models
{
    public class Recipe
    {
        // 🔑 PK
        public long RecipeId { get; set; }

        // 📄 Dados principais
        [Required(ErrorMessage = "O título é obrigatório.")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "O título deve ter entre 3 e 100 caracteres.")]
        public string Title { get; set; } = string.Empty;

        [Required(ErrorMessage = "O modo de preparação é obrigatório.")]
        [StringLength(2000, ErrorMessage = "O modo de preparação não pode exceder os 2000 caracteres.")]
        public string PreparationMethod { get; set; } = string.Empty;

        [Required(ErrorMessage = "O tempo de preparação é obrigatório.")]
        [Range(1, 1440, ErrorMessage = "O tempo deve ser entre 1 minuto e 24 horas.")]
        public int PreparationTime { get; set; }

        // 🔗 FK
        [Required(ErrorMessage = "Selecione uma categoria.")]
        public int CategoryId { get; set; }

        [Required(ErrorMessage = "Selecione uma dificuldade.")]
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

        // Favoritos ❤️ ADDCOUNT
        public int IngredientsCount { get; set; }

        // 🥗 RELACIONAMENTOS 
        public List<Ingredient> Ingredients { get; set; } = new List<Ingredient>();
    }
}