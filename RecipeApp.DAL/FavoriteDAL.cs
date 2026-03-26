using Microsoft.Data.SqlClient;
using RecipeApp.Models;
using System.Collections.Generic;

namespace RecipeApp.DAL
{
    public class FavoriteDAL
    {
        private readonly DbHelper _db;

        public FavoriteDAL(DbHelper db)
        {
            _db = db;
        }

        public List<Recipe> GetFavoritesByUserId(long userId)
        {
            var recipes = new List<Recipe>();
            using var connection = _db.GetConnection();
            //  JOIN para trazer os dados da receita junto com o favorito
            string sql = @"SELECT r.*, c.Name AS CategoryName, d.Name AS DifficultyName 
                           FROM Recipe r
                           INNER JOIN Favourite f ON r.RecipeId = f.RecipeId
                           INNER JOIN Category c ON r.CategoryId = c.CategoryId
                           INNER JOIN Difficulty d ON r.DifficultyId = d.DifficultyId
                           WHERE f.UserId = @UserId";

            using var cmd = new SqlCommand(sql, connection);
            cmd.Parameters.AddWithValue("@UserId", userId);
            connection.Open();
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                recipes.Add(new Recipe
                {
                    RecipeId = (long)reader["RecipeId"],
                    Title = reader["Title"].ToString() ?? "",
                    CategoryName = reader["CategoryName"].ToString() ?? "",
                    DifficultyName = reader["DifficultyName"].ToString() ?? "",
                    IsFavorite = true // Se está nesta lista, é favorito
                });
            }
            return recipes;
        }

        public void DeleteFavorite(long userId, long recipeId)
        {
            using var connection = _db.GetConnection();
            string sql = "DELETE FROM Favourite WHERE UserId = @Uid AND RecipeId = @Rid";
            using var cmd = new SqlCommand(sql, connection);
            cmd.Parameters.AddWithValue("@Uid", userId);
            cmd.Parameters.AddWithValue("@Rid", recipeId);
            connection.Open();
            cmd.ExecuteNonQuery();
        }

        public void InsertFavorite(long userId, long recipeId)
        {
            using var connection = _db.GetConnection();
            string sql = "INSERT INTO Favourite (UserId, RecipeId) VALUES (@Uid, @Rid)";
            using var cmd = new SqlCommand(sql, connection);
            cmd.Parameters.AddWithValue("@Uid", userId);
            cmd.Parameters.AddWithValue("@Rid", recipeId);
            connection.Open();
            cmd.ExecuteNonQuery();
        }
    }
}