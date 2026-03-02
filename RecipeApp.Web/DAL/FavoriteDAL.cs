using Microsoft.Data.SqlClient;
using RecipeApp.Web.Models;
using Microsoft.Extensions.Configuration;

namespace RecipeApp.Web.DAL
{
    public class FavoriteDAL
    {
        private readonly DbHelper _db;

        public FavoriteDAL(DbHelper db)
        {
            _db = db;
        }

        // ✅ Adicionar aos favoritos
        public void Add(long userId, long recipeId)
        {
            using var connection = _db.GetConnection();

            // Usamos [Favourite] com 'u' e parênteses retos para evitar conflitos de nomes no SQL
            string sql = @"
                INSERT INTO [Favourite] (UserId, RecipeId, CreatedAt)
                VALUES (@UserId, @RecipeId, GETDATE())
            ";

            using var cmd = new SqlCommand(sql, connection);
            cmd.Parameters.AddWithValue("@UserId", userId);
            cmd.Parameters.AddWithValue("@RecipeId", recipeId);

            connection.Open();
            cmd.ExecuteNonQuery();
        }

        // ❌ Remover dos favoritos
        public void Remove(long userId, long recipeId)
        {
            using var connection = _db.GetConnection();

            string sql = @"
                DELETE FROM [Favourite]
                WHERE UserId = @UserId AND RecipeId = @RecipeId
            ";

            using var cmd = new SqlCommand(sql, connection);
            cmd.Parameters.AddWithValue("@UserId", userId);
            cmd.Parameters.AddWithValue("@RecipeId", recipeId);

            connection.Open();
            cmd.ExecuteNonQuery();
        }

        // ⭐ Verificar se a receita já é favorita do utilizador
        public bool IsFavorite(long userId, long recipeId)
        {
            using var connection = _db.GetConnection();

            string sql = @"
                SELECT COUNT(1)
                FROM [Favourite]
                WHERE UserId = @UserId AND RecipeId = @RecipeId
            ";

            using var cmd = new SqlCommand(sql, connection);
            cmd.Parameters.AddWithValue("@UserId", userId);
            cmd.Parameters.AddWithValue("@RecipeId", recipeId);

            connection.Open();
            // Retorna true se encontrar pelo menos 1 registo
            return (int)cmd.ExecuteScalar() > 0;
        }

        // 📋 Listar todas as receitas favoritas de um utilizador específico
        public List<Recipe> GetUserFavorites(long userId)
        {
            var list = new List<Recipe>();
            using var connection = _db.GetConnection();

            string sql = @"
                SELECT r.RecipeId, r.Title
                FROM [Favourite] f
                INNER JOIN Recipe r ON f.RecipeId = r.RecipeId
                WHERE f.UserId = @UserId
            ";

            using var cmd = new SqlCommand(sql, connection);
            cmd.Parameters.AddWithValue("@UserId", userId);

            connection.Open();
            using var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                list.Add(new Recipe
                {
                    RecipeId = (long)reader["RecipeId"],
                    Title = reader["Title"].ToString()
                });
            }

            return list;
        }
    }
}