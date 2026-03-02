using Microsoft.Data.SqlClient;

namespace RecipeApp.Web.DAL
{
    public class RatingDAL
    {
        private readonly DbHelper _db;

        public RatingDAL(DbHelper db)
        {
            _db = db;
        }

        // Inserir ou atualizar rating
        public void SaveRating(long recipeId, long userId, int value)
        {
            using var connection = _db.GetConnection();

            string sql = @"
                IF EXISTS (
                    SELECT 1 FROM Rating
                    WHERE RecipeId = @RecipeId AND UserId = @UserId
                )
                BEGIN
                    UPDATE Rating
                    SET Value = @Value
                    WHERE RecipeId = @RecipeId AND UserId = @UserId
                END
                ELSE
                BEGIN
                    INSERT INTO Rating (RecipeId, UserId, Value, CreatedAt)
                    VALUES (@RecipeId, @UserId, @Value, GETDATE())
                END
            ";

            using var cmd = new SqlCommand(sql, connection);
            cmd.Parameters.AddWithValue("@RecipeId", recipeId);
            cmd.Parameters.AddWithValue("@UserId", userId);
            cmd.Parameters.AddWithValue("@Value", value);

            connection.Open();
            cmd.ExecuteNonQuery();
        }

        // Média da receita
        public double GetAverageRating(long recipeId)
        {
            using var connection = _db.GetConnection();

            string sql = @"
                SELECT AVG(CAST(Value AS FLOAT))
                FROM Rating
                WHERE RecipeId = @RecipeId
            ";

            using var cmd = new SqlCommand(sql, connection);
            cmd.Parameters.AddWithValue("@RecipeId", recipeId);

            connection.Open();
            var result = cmd.ExecuteScalar();

            return result == DBNull.Value ? 0 : (double)result;
        }
    }
}
