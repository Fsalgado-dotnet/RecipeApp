using Microsoft.Data.SqlClient;
using RecipeApp.Web.Models;
using System;
using System.Collections.Generic;

namespace RecipeApp.Web.DAL
{
    public class RecipeDAL
    {
        private readonly DbHelper _db;

        public RecipeDAL(DbHelper db)
        {
            _db = db;
        }

        // ============================================================
        // LISTAR RECEITAS APROVADAS (INDEX) - COM FILTROS DINÂMICOS
        // ============================================================
        public List<Recipe> GetApprovedRecipes(long? userId = null, string searchTerm = null, long? categoryId = null, long? difficultyId = null)
        {
            var recipes = new List<Recipe>();
            using var connection = _db.GetConnection();

            string sql = @"
                SELECT 
                    r.RecipeId,
                    r.Title,
                    r.PreparationTime,
                    r.IsApproved,
                    r.CreatedAt,
                    r.CreatedByUserId,
                    c.Name AS CategoryName,
                    d.Name AS DifficultyName,
                    ISNULL(AVG(CAST(rt.Value AS FLOAT)), 0) AS AverageRating,
                    CASE 
                        WHEN f.UserId IS NULL THEN 0 
                        ELSE 1 
                    END AS IsFavorite
                FROM Recipe r
                INNER JOIN Category c ON r.CategoryId = c.CategoryId
                INNER JOIN Difficulty d ON r.DifficultyId = d.DifficultyId
                LEFT JOIN Rating rt ON r.RecipeId = rt.RecipeId
                LEFT JOIN Favourite f 
                    ON r.RecipeId = f.RecipeId
                    AND f.UserId = @UserId
                WHERE r.IsApproved = 1 "; // Só mostra as que estão aprovadas (e não apagadas)

            // --- Injeção de Filtros Dinâmicos ---
            if (!string.IsNullOrEmpty(searchTerm))
            {
                sql += " AND r.Title LIKE @Search ";
            }

            if (categoryId.HasValue && categoryId > 0)
            {
                sql += " AND r.CategoryId = @CatId ";
            }

            if (difficultyId.HasValue && difficultyId > 0)
            {
                sql += " AND r.DifficultyId = @DiffId ";
            }

            sql += @" GROUP BY 
                        r.RecipeId,
                        r.Title,
                        r.PreparationTime,
                        r.IsApproved,
                        r.CreatedAt,
                        r.CreatedByUserId,
                        c.Name,
                        d.Name,
                        f.UserId
                    ORDER BY r.CreatedAt DESC";

            using var cmd = new SqlCommand(sql, connection);

            // Parâmetros base
            cmd.Parameters.AddWithValue("@UserId", (object?)userId ?? DBNull.Value);

            // Parâmetros Condicionais
            if (!string.IsNullOrEmpty(searchTerm))
            {
                cmd.Parameters.AddWithValue("@Search", "%" + searchTerm + "%");
            }

            if (categoryId.HasValue && categoryId > 0)
            {
                cmd.Parameters.AddWithValue("@CatId", categoryId.Value);
            }

            if (difficultyId.HasValue && difficultyId > 0)
            {
                cmd.Parameters.AddWithValue("@DiffId", difficultyId.Value);
            }

            connection.Open();
            using var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                recipes.Add(new Recipe
                {
                    RecipeId = (long)reader["RecipeId"],
                    Title = reader["Title"]?.ToString() ?? string.Empty,
                    PreparationTime = (int)reader["PreparationTime"],
                    CategoryName = reader["CategoryName"]?.ToString() ?? string.Empty,
                    DifficultyName = reader["DifficultyName"]?.ToString() ?? string.Empty,
                    AverageRating = Convert.ToDouble(reader["AverageRating"]),
                    IsFavorite = Convert.ToInt32(reader["IsFavorite"]) == 1,
                    CreatedByUserId = (long)reader["CreatedByUserId"] // Necessário para o botão apagar
                });
            }

            return recipes;
        }

        // ===============================
        // DETALHE DA RECEITA
        // ===============================
        public Recipe? GetRecipeById(long recipeId)
        {
            using var connection = _db.GetConnection();

            string sql = @"
                SELECT 
                    r.RecipeId,
                    r.Title,
                    CAST(r.PreparationMethod AS NVARCHAR(MAX)) AS PreparationMethod,
                    r.PreparationTime,
                    r.CreatedAt,
                    r.CreatedByUserId,
                    c.Name AS CategoryName,
                    d.Name AS DifficultyName,
                    ISNULL(AVG(CAST(rt.Value AS FLOAT)), 0) AS AverageRating
                FROM Recipe r
                INNER JOIN Category c ON r.CategoryId = c.CategoryId
                INNER JOIN Difficulty d ON r.DifficultyId = d.DifficultyId
                LEFT JOIN Rating rt ON r.RecipeId = rt.RecipeId
                WHERE r.RecipeId = @RecipeId
                GROUP BY
                    r.RecipeId,
                    r.Title,
                    CAST(r.PreparationMethod AS NVARCHAR(MAX)),
                    r.PreparationTime,
                    r.CreatedAt,
                    r.CreatedByUserId,
                    c.Name,
                    d.Name
            ";

            using var cmd = new SqlCommand(sql, connection);
            cmd.Parameters.AddWithValue("@RecipeId", recipeId);

            connection.Open();
            using var reader = cmd.ExecuteReader();

            if (!reader.Read())
                return null;

            return new Recipe
            {
                RecipeId = (long)reader["RecipeId"],
                Title = reader["Title"]?.ToString() ?? string.Empty,
                PreparationMethod = reader["PreparationMethod"]?.ToString() ?? string.Empty,
                PreparationTime = (int)reader["PreparationTime"],
                CategoryName = reader["CategoryName"]?.ToString() ?? string.Empty,
                DifficultyName = reader["DifficultyName"]?.ToString() ?? string.Empty,
                AverageRating = Convert.ToDouble(reader["AverageRating"]),
                CreatedAt = (DateTime)reader["CreatedAt"],
                CreatedByUserId = (long)reader["CreatedByUserId"]
            };
        }

        // ===============================
        // RECEITAS DO UTILIZADOR
        // ===============================
        public List<Recipe> GetByUser(long userId)
        {
            var list = new List<Recipe>();
            using var connection = _db.GetConnection();

            string sql = @"
                SELECT 
                    RecipeId,
                    Title,
                    IsApproved,
                    CreatedAt,
                    CreatedByUserId
                FROM Recipe
                WHERE CreatedByUserId = @UserId
                ORDER BY CreatedAt DESC
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
                    Title = reader["Title"]?.ToString() ?? string.Empty,
                    IsApproved = (bool)reader["IsApproved"],
                    CreatedAt = (DateTime)reader["CreatedAt"],
                    CreatedByUserId = (long)reader["CreatedByUserId"]
                });
            }

            return list;
        }

        // ============================================================
        // ELIMINAR RECEITA (SOFT DELETE)
        // Só permite se o UserId coincidir com o criador
        // ============================================================
        public bool DeleteRecipe(long recipeId, long userId)
        {
            using var connection = _db.GetConnection();

            // Mudamos IsApproved para 0. A receita continua na DB, mas sai do Index.
            string sql = @"
                UPDATE Recipe 
                SET IsApproved = 0 
                WHERE RecipeId = @RecipeId AND CreatedByUserId = @UserId";

            using var cmd = new SqlCommand(sql, connection);
            cmd.Parameters.AddWithValue("@RecipeId", recipeId);
            cmd.Parameters.AddWithValue("@UserId", userId);

            connection.Open();
            int rowsAffected = cmd.ExecuteNonQuery();

            return rowsAffected > 0;
        }

        // ===============================
        // CRIAR RECEITA
        // ===============================
        public void CreateRecipe(Recipe recipe)
        {
            using var connection = _db.GetConnection();

            string sql = @"
                INSERT INTO Recipe
                (Title, PreparationMethod, PreparationTime, CategoryId, DifficultyId, CreatedByUserId, IsApproved, CreatedAt)
                VALUES
                (@Title, @Method, @Time, @CategoryId, @DifficultyId, @UserId, @Approved, GETDATE())
            ";

            using var cmd = new SqlCommand(sql, connection);
            cmd.Parameters.AddWithValue("@Title", recipe.Title);
            cmd.Parameters.AddWithValue("@Method", recipe.PreparationMethod);
            cmd.Parameters.AddWithValue("@Time", recipe.PreparationTime);
            cmd.Parameters.AddWithValue("@CategoryId", recipe.CategoryId);
            cmd.Parameters.AddWithValue("@DifficultyId", recipe.DifficultyId);
            cmd.Parameters.AddWithValue("@UserId", recipe.CreatedByUserId);
            cmd.Parameters.AddWithValue("@Approved", recipe.IsApproved);

            connection.Open();
            cmd.ExecuteNonQuery();
        }

        // ===============================
        // APROVAR RECEITA (ADMIN)
        // ===============================
        public void ApproveRecipe(long recipeId)
        {
            using var connection = _db.GetConnection();

            string sql = @"
                UPDATE Recipe
                SET IsApproved = 1
                WHERE RecipeId = @RecipeId
            ";

            using var cmd = new SqlCommand(sql, connection);
            cmd.Parameters.AddWithValue("@RecipeId", recipeId);

            connection.Open();
            cmd.ExecuteNonQuery();
        }

        // ===============================
        // LISTAR RECEITAS PENDENTES (ADMIN)
        // ===============================
        public List<Recipe> GetPendingRecipes()
        {
            var recipes = new List<Recipe>();
            using var connection = _db.GetConnection();

            string sql = @"
                SELECT 
                    r.RecipeId,
                    r.Title,
                    r.CreatedAt,
                    u.Name AS AuthorName
                FROM Recipe r
                INNER JOIN Users u ON r.CreatedByUserId = u.UserId
                WHERE r.IsApproved = 0
                ORDER BY r.CreatedAt DESC
            ";

            using var cmd = new SqlCommand(sql, connection);
            connection.Open();

            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                recipes.Add(new Recipe
                {
                    RecipeId = (long)reader["RecipeId"],
                    Title = reader["Title"]?.ToString() ?? string.Empty,
                    CreatedAt = (DateTime)reader["CreatedAt"]
                });
            }

            return recipes;
        }

        // ===============================
        // EDITAR RECEITA
        // ===============================
        public void Update(Recipe recipe)
        {
            using var connection = _db.GetConnection();

            string sql = @"
                UPDATE Recipe
                SET
                    Title = @Title,
                    PreparationMethod = @Method,
                    PreparationTime = @Time,
                    CategoryId = @CategoryId,
                    DifficultyId = @DifficultyId
                WHERE RecipeId = @RecipeId
            ";

            using var cmd = new SqlCommand(sql, connection);
            cmd.Parameters.AddWithValue("@Title", recipe.Title);
            cmd.Parameters.AddWithValue("@Method", recipe.PreparationMethod);
            cmd.Parameters.AddWithValue("@Time", recipe.PreparationTime);
            cmd.Parameters.AddWithValue("@CategoryId", recipe.CategoryId);
            cmd.Parameters.AddWithValue("@DifficultyId", recipe.DifficultyId);
            cmd.Parameters.AddWithValue("@RecipeId", recipe.RecipeId);

            connection.Open();
            cmd.ExecuteNonQuery();
        }
    }
}