namespace RecipeApp.Web.Models
{
    public class Ingredient
    {
        public long IngredientId { get; set; }
        public string Name { get; set; } = string.Empty;

        
        public string IngredientName { get { return Name; } set { Name = value; } }

      
        public string Unit { get; set; } = string.Empty;

        public string Quantity { get; set; } = string.Empty;
    }
}