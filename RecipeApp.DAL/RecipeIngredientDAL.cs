using Microsoft.Data.SqlClient;
using RecipeApp.Models;

namespace RecipeApp.DAL
{
    public class RecipeIngredientDAL
    {
        private readonly DbHelper _db;

        public RecipeIngredientDAL(DbHelper db)
        {
            _db = db;
        }

        public void Add(RecipeIngredient ri)
        {
            using var connection = _db.GetConnection();
            string sql = @"
                INSERT INTO RecipeIngredient (RecipeId, IngredientId, Quantity, Unit)
                VALUES (@RecipeId, @IngredientId, @Quantity, @Unit)";

            using var cmd = new SqlCommand(sql, connection);
            cmd.Parameters.AddWithValue("@RecipeId", ri.RecipeId);
            cmd.Parameters.AddWithValue("@IngredientId", ri.IngredientId);
            cmd.Parameters.AddWithValue("@Quantity", ri.Quantity);
            cmd.Parameters.AddWithValue("@Unit", ri.Unit ?? (object)DBNull.Value);

            connection.Open();
            cmd.ExecuteNonQuery();
        }

        public void Remove(long recipeId, long ingredientId)
        {
            using var connection = _db.GetConnection();
            string sql = "DELETE FROM RecipeIngredient WHERE RecipeId = @RecipeId AND IngredientId = @IngredientId";

            using var cmd = new SqlCommand(sql, connection);
            cmd.Parameters.AddWithValue("@RecipeId", recipeId);
            cmd.Parameters.AddWithValue("@IngredientId", ingredientId);

            connection.Open();
            cmd.ExecuteNonQuery();
        }

        public List<RecipeIngredient> GetByRecipe(long recipeId)
        {
            var list = new List<RecipeIngredient>();
            try
            {
                using var connection = _db.GetConnection();
                string sql = @"
                    SELECT ri.RecipeId, ri.IngredientId, ri.Quantity, ri.Unit, i.Name AS IngredientName
                    FROM RecipeIngredient ri
                    INNER JOIN Ingredient i ON ri.IngredientId = i.IngredientId
                    WHERE ri.RecipeId = @RecipeId";

                using var cmd = new SqlCommand(sql, connection);
                cmd.Parameters.AddWithValue("@RecipeId", recipeId);

                connection.Open();
                using var reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    list.Add(new RecipeIngredient
                    {
                        RecipeId = Convert.ToInt64(reader["RecipeId"]),
                        IngredientId = Convert.ToInt64(reader["IngredientId"]),
                        Quantity = Convert.ToDouble(reader["Quantity"]),
                        Unit = reader["Unit"]?.ToString() ?? "",
                        IngredientName = reader["IngredientName"]?.ToString() ?? ""
                    });
                }
            }
            catch
            {
                //  Retorna lista vazia se a BD falhar
            }
            return list;
        }
    }
}