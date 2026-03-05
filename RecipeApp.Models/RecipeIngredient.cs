namespace RecipeApp.Models
{
    public class RecipeIngredient
    {
        public long RecipeId { get; set; }
        public long IngredientId { get; set; }

        public double Quantity { get; set; }
        public string Unit { get; set; } = string.Empty;

        // opcional (para UI)
        public string IngredientName { get; set; } = string.Empty;
    }
}

