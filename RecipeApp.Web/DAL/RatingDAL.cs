using Microsoft.Data.SqlClient;
using System;

namespace RecipeApp.Web.DAL
{
    public class RatingDAL
    {
        private readonly DbHelper _db;

        public RatingDAL(DbHelper db)
        {
            _db = db;
        }

        public void SaveRating(long recipeId, long userId, int value)
        {
            using var connection = _db.GetConnection();

            // CORREÇÃO: Removida a coluna 'CreatedAt' que causava o erro SqlException
            // A tua tabela Rating só tem: RatingId, RecipeId, UserId, Value
            string sql = @"
                IF EXISTS (SELECT 1 FROM Rating WHERE RecipeId = @RecipeId AND UserId = @UserId)
                BEGIN
                    UPDATE Rating SET Value = @Value WHERE RecipeId = @RecipeId AND UserId = @UserId
                END
                ELSE
                BEGIN
                    INSERT INTO Rating (RecipeId, UserId, Value)
                    VALUES (@RecipeId, @UserId, @Value)
                END";

            using var cmd = new SqlCommand(sql, connection);
            cmd.Parameters.AddWithValue("@RecipeId", recipeId);
            cmd.Parameters.AddWithValue("@UserId", userId);
            cmd.Parameters.AddWithValue("@Value", value);

            connection.Open();
            // Já não vai "rebentar" aqui
            cmd.ExecuteNonQuery();
        }

        public double GetAverageRating(long recipeId)
        {
            try
            {
                using var connection = _db.GetConnection();
                string sql = "SELECT AVG(CAST(Value AS FLOAT)) FROM Rating WHERE RecipeId = @RecipeId";

                using var cmd = new SqlCommand(sql, connection);
                cmd.Parameters.AddWithValue("@RecipeId", recipeId);

                connection.Open();
                var result = cmd.ExecuteScalar();

                return result == DBNull.Value ? 0 : Convert.ToDouble(result);
            }
            catch
            {
                return 0; // Proteção para a página não crashar se houver erro de leitura
            }
        }
    }
}