using Microsoft.Data.SqlClient;
using RecipeApp.Models;
using System;
using System.Collections.Generic;
using System.Data;

namespace RecipeApp.DAL
{
    public class RecipeDAL
    {
        private readonly DbHelper _db;

        public RecipeDAL(DbHelper db)
        {
            _db = db;
        }

        // --- GESTÃO DE INGREDIENTES ---

        public long GetOrCreateIngredient(string name) // verifica se o nome existe , se nao cria um novo e devolve ID
        {
            using var connection = _db.GetConnection(); // garante que a ligação é fechadao auto quando terminamos mesmo que de erro
            connection.Open();

            string selectSql = "SELECT IngredientId FROM Ingredient WHERE Name = @Name";
            using var selectCmd = new SqlCommand(selectSql, connection);
            selectCmd.Parameters.AddWithValue("@Name", name.Trim());

            var result = selectCmd.ExecuteScalar();
            if (result != null && result != DBNull.Value)
            {
                return Convert.ToInt64(result);
            }

            string insertSql = "INSERT INTO Ingredient (Name) VALUES (@Name); SELECT CAST(SCOPE_IDENTITY() AS BIGINT);";
            using var insertCmd = new SqlCommand(insertSql, connection);
            insertCmd.Parameters.AddWithValue("@Name", name.Trim());

            return (long)insertCmd.ExecuteScalar();
        }

        public void AddIngredientToRecipe(long rid, long iid, string qty)
        {
            using var connection = _db.GetConnection();
            string sql = @"
                IF NOT EXISTS (SELECT 1 FROM RecipeIngredient WHERE RecipeId = @R AND IngredientId = @I)
                BEGIN
                    INSERT INTO RecipeIngredient (RecipeId, IngredientId, Quantity) VALUES (@R, @I, @Q)
                END
                ELSE
                BEGIN
                    UPDATE RecipeIngredient SET Quantity = @Q WHERE RecipeId = @R AND IngredientId = @I
                END";

            using var cmd = new SqlCommand(sql, connection); //Sql Command é o objecto que transporta a nossa query
            cmd.Parameters.AddWithValue("@R", rid);          // sql até ao servidor
            cmd.Parameters.AddWithValue("@I", iid);
            cmd.Parameters.AddWithValue("@Q", qty);  // cmd.parameteres - protege sql injection dos users , nunca concatenamos strings nas querys
            connection.Open();                       // define logo e converte os dados colocados (strings...)
            cmd.ExecuteNonQuery();
        }

        public List<Ingredient> GetIngredientsByRecipeId(long id)
        {
            var list = new List<Ingredient>();
            using var connection = _db.GetConnection();
            string sql = @"SELECT i.IngredientId, i.Name, ri.Quantity 
                           FROM Ingredient i 
                           INNER JOIN RecipeIngredient ri ON i.IngredientId = ri.IngredientId 
                           WHERE ri.RecipeId = @Id";
            using var cmd = new SqlCommand(sql, connection);
            cmd.Parameters.AddWithValue("@Id", id);
            connection.Open();
            using var reader = cmd.ExecuteReader();  // consultas a BD (select) para retornar um cursor linha a linha.
            while (reader.Read())
            {
                list.Add(new Ingredient
                {
                    IngredientId = Convert.ToInt64(reader["IngredientId"]),
                    Name = reader["Name"]?.ToString() ?? "",
                    Quantity = reader["Quantity"]?.ToString() ?? ""
                });
            }
            return list;
        }

        public void DeleteIngredientFromRecipe(long rid, long iid)
        {
            using var connection = _db.GetConnection();
            using var cmd = new SqlCommand("DELETE FROM RecipeIngredient WHERE RecipeId=@R AND IngredientId=@I", connection);
            cmd.Parameters.AddWithValue("@R", rid);
            cmd.Parameters.AddWithValue("@I", iid);
            connection.Open();
            cmd.ExecuteNonQuery();  // usado quando nao retorna dados (insert, update, delete)
        }

        // --- FAVORITOS ---

        public List<Recipe> GetFavoriteRecipes(long userId)
        {
            var recipes = new List<Recipe>();
            using var connection = _db.GetConnection();
            string sql = @"
                SELECT r.*, c.Name AS CategoryName, d.Name AS DifficultyName,
                       1 AS IsFavorite,
                       ISNULL((SELECT AVG(CAST(Value AS FLOAT)) FROM Rating WHERE RecipeId = r.RecipeId), 0) AS AverageRating,
                       (SELECT COUNT(*) FROM RecipeIngredient WHERE RecipeId = r.RecipeId) AS IngredientsCount
                FROM Recipe r
                INNER JOIN Favourite f ON r.RecipeId = f.RecipeId
                INNER JOIN Category c ON r.CategoryId = c.CategoryId
                INNER JOIN Difficulty d ON r.DifficultyId = d.DifficultyId
                WHERE f.UserId = @UserId AND r.IsApproved = 1
                ORDER BY r.CreatedAt DESC";

            using var cmd = new SqlCommand(sql, connection);
            cmd.Parameters.AddWithValue("@UserId", userId);
            connection.Open();
            using var reader = cmd.ExecuteReader();
            while (reader.Read()) { recipes.Add(MapFromReader(reader)); }
            return recipes;
        }

        public void ToggleFavorite(long userId, long recipeId)
        {
            using var connection = _db.GetConnection();
            string sql = @"
                IF EXISTS (SELECT 1 FROM Favourite WHERE UserId = @UserId AND RecipeId = @RecipeId)
                BEGIN
                    DELETE FROM Favourite WHERE UserId = @UserId AND RecipeId = @RecipeId
                END
                ELSE
                BEGIN
                    INSERT INTO Favourite (UserId, RecipeId) VALUES (@UserId, @RecipeId)
                END";

            using var cmd = new SqlCommand(sql, connection);
            cmd.Parameters.AddWithValue("@UserId", userId);
            cmd.Parameters.AddWithValue("@RecipeId", recipeId);
            connection.Open();
            cmd.ExecuteNonQuery();
        }

        // --- LISTAGEM ---

        public List<Recipe> GetApprovedRecipes(long? userId, string? searchTerm, long? categoryId, long? difficultyId, string sort = "Todas")
        {
            var recipes = new List<Recipe>();
            using var connection = _db.GetConnection();
            string sql = @"
                SELECT r.*, c.Name AS CategoryName, d.Name AS DifficultyName,
                       ISNULL((SELECT AVG(CAST(Value AS FLOAT)) FROM Rating WHERE RecipeId = r.RecipeId), 0) AS AverageRating,
                       CASE WHEN f.UserId IS NULL THEN 0 ELSE 1 END AS IsFavorite,
                       (SELECT COUNT(*) FROM RecipeIngredient WHERE RecipeId = r.RecipeId) AS IngredientsCount
                FROM Recipe r
                INNER JOIN Category c ON r.CategoryId = c.CategoryId
                INNER JOIN Difficulty d ON r.DifficultyId = d.DifficultyId
                LEFT JOIN Favourite f ON r.RecipeId = f.RecipeId AND f.UserId = @UserId
                WHERE r.IsApproved = 1 ";

            if (!string.IsNullOrEmpty(searchTerm)) sql += " AND r.Title LIKE @Search ";
            if (categoryId > 0) sql += " AND r.CategoryId = @CatId ";
            if (difficultyId > 0) sql += " AND r.DifficultyId = @DiffId ";

            if (sort == "Populares") sql += " ORDER BY AverageRating DESC ";
            else if (sort == "Novas") sql += " ORDER BY r.CreatedAt DESC ";
            else sql += " ORDER BY r.Title ASC ";

            using var cmd = new SqlCommand(sql, connection);
            cmd.Parameters.AddWithValue("@UserId", (object?)userId ?? DBNull.Value);
            if (!string.IsNullOrEmpty(searchTerm)) cmd.Parameters.AddWithValue("@Search", "%" + searchTerm + "%");
            if (categoryId > 0) cmd.Parameters.AddWithValue("@CatId", categoryId.Value);
            if (difficultyId > 0) cmd.Parameters.AddWithValue("@DiffId", difficultyId.Value);

            connection.Open();
            using var reader = cmd.ExecuteReader();
            while (reader.Read()) { recipes.Add(MapFromReader(reader)); }
            return recipes;
        }

        // --- GESTÃO E ADMIN ---

        public List<Recipe> GetPendingRecipes()
        {
            var recipes = new List<Recipe>();
            using var connection = _db.GetConnection();
            string sql = @"
                SELECT r.*, c.Name AS CategoryName, d.Name AS DifficultyName, 
                       0 AS AverageRating, 0 AS IsFavorite,
                       (SELECT COUNT(*) FROM RecipeIngredient WHERE RecipeId = r.RecipeId) AS IngredientsCount
                FROM Recipe r
                INNER JOIN Category c ON r.CategoryId = c.CategoryId
                INNER JOIN Difficulty d ON r.DifficultyId = d.DifficultyId
                WHERE r.IsApproved = 0 
                     AND (r.RejectionReason IS NULL OR r.RejectionReason = '')
                ORDER BY r.CreatedAt ASC";

            using var cmd = new SqlCommand(sql, connection);
            connection.Open();
            using var reader = cmd.ExecuteReader();
            while (reader.Read()) { recipes.Add(MapFromReader(reader)); }
            return recipes;
        }

        public List<Recipe> GetRecipesByUserId(long userId)
        {
            var recipes = new List<Recipe>();
            using var connection = _db.GetConnection();
            string sql = @"
                SELECT r.*, c.Name AS CategoryName, d.Name AS DifficultyName, 
                       ISNULL((SELECT AVG(CAST(Value AS FLOAT)) FROM Rating WHERE RecipeId = r.RecipeId), 0) AS AverageRating,
                       CASE WHEN (SELECT 1 FROM Favourite WHERE UserId = @UserId AND RecipeId = r.RecipeId) IS NULL THEN 0 ELSE 1 END AS IsFavorite,
                       (SELECT COUNT(*) FROM RecipeIngredient WHERE RecipeId = r.RecipeId) AS IngredientsCount
                FROM Recipe r
                INNER JOIN Category c ON r.CategoryId = c.CategoryId
                INNER JOIN Difficulty d ON r.DifficultyId = d.DifficultyId
                WHERE r.CreatedByUserId = @UserId AND r.IsApproved >= 0
                ORDER BY r.CreatedAt DESC";

            using var cmd = new SqlCommand(sql, connection);
            cmd.Parameters.AddWithValue("@UserId", userId);
            connection.Open();
            using var reader = cmd.ExecuteReader();
            while (reader.Read()) { recipes.Add(MapFromReader(reader)); }
            return recipes;
        }

        public Recipe? GetRecipeById(long id)
        {
            using var connection = _db.GetConnection();
            string sql = @"
                SELECT r.*, c.Name AS CategoryName, d.Name AS DifficultyName, 
                ISNULL((SELECT AVG(CAST(Value AS FLOAT)) FROM Rating WHERE RecipeId = r.RecipeId), 0) AS AverageRating,
                0 AS IsFavorite,
                (SELECT COUNT(*) FROM RecipeIngredient WHERE RecipeId = r.RecipeId) AS IngredientsCount
                FROM Recipe r 
                INNER JOIN Category c ON r.CategoryId = c.CategoryId 
                INNER JOIN Difficulty d ON r.DifficultyId = d.DifficultyId 
                WHERE r.RecipeId = @Id";

            using var cmd = new SqlCommand(sql, connection);
            cmd.Parameters.AddWithValue("@Id", id);
            connection.Open();
            using var reader = cmd.ExecuteReader();
            return reader.Read() ? MapFromReader(reader) : null;
        }

        public int CreateRecipe(Recipe r)
        {
            using var connection = _db.GetConnection();
            string sql = @"INSERT INTO Recipe (Title, PreparationMethod, PreparationTime, CategoryId, DifficultyId, CreatedByUserId, IsApproved, CreatedAt) 
                           VALUES (@T, @M, @P, @C, @D, @U, 0, GETDATE());
                           SELECT CAST(SCOPE_IDENTITY() AS INT);";

            using var cmd = new SqlCommand(sql, connection);
            cmd.Parameters.AddWithValue("@T", r.Title);
            cmd.Parameters.AddWithValue("@M", r.PreparationMethod);
            cmd.Parameters.AddWithValue("@P", r.PreparationTime);
            cmd.Parameters.AddWithValue("@C", r.CategoryId);
            cmd.Parameters.AddWithValue("@D", r.DifficultyId);
            cmd.Parameters.AddWithValue("@U", r.CreatedByUserId);

            connection.Open();
            return (int)cmd.ExecuteScalar();
        }

        public void UpdateRecipe(Recipe r)
        {
            using var connection = _db.GetConnection();
            string sql = "UPDATE Recipe SET Title=@T, PreparationMethod=@M, PreparationTime=@P, CategoryId=@C, DifficultyId=@D, RejectionReason = NULL WHERE RecipeId=@Id";
            using var cmd = new SqlCommand(sql, connection);
            cmd.Parameters.AddWithValue("@T", r.Title);
            cmd.Parameters.AddWithValue("@M", r.PreparationMethod);
            cmd.Parameters.AddWithValue("@P", r.PreparationTime);
            cmd.Parameters.AddWithValue("@C", r.CategoryId);
            cmd.Parameters.AddWithValue("@D", r.DifficultyId);
            cmd.Parameters.AddWithValue("@Id", r.RecipeId);
            connection.Open();
            cmd.ExecuteNonQuery();
        }

        public bool DeleteRecipe(long recipeId, long userId)
        {
            using var connection = _db.GetConnection();
            connection.Open();
            using var transaction = connection.BeginTransaction();
            try
            {
                string sqlCleanup = @"
                    DELETE FROM RecipeIngredient WHERE RecipeId = @Rid;
                    DELETE FROM Favourite WHERE RecipeId = @Rid;
                    DELETE FROM Rating WHERE RecipeId = @Rid;
                    DELETE FROM Comment WHERE RecipeId = @Rid;
                    DELETE FROM Recipe WHERE RecipeId = @Rid AND CreatedByUserId = @Uid;";
                using var cmd = new SqlCommand(sqlCleanup, connection, transaction);
                cmd.Parameters.AddWithValue("@Rid", recipeId);
                cmd.Parameters.AddWithValue("@Uid", userId);
                int rows = cmd.ExecuteNonQuery();
                transaction.Commit();
                return rows > 0;
            }
            catch { transaction.Rollback(); throw; }
        }

        // limpa o motivo ao aprovar
        public void ApproveRecipe(long id)
        {
            using var connection = _db.GetConnection();
            using var cmd = new SqlCommand("UPDATE Recipe SET IsApproved = 1, RejectionReason = NULL WHERE RecipeId = @Id", connection);
            cmd.Parameters.AddWithValue("@Id", id);
            connection.Open();
            cmd.ExecuteNonQuery();
        }

        //  RejectRecipe no Service
        public void RejectRecipe(long id, string reason)
        {
            using var connection = _db.GetConnection();
            using var cmd = new SqlCommand("UPDATE Recipe SET IsApproved = 0, RejectionReason = @Reason WHERE RecipeId = @Id", connection);
            cmd.Parameters.AddWithValue("@Reason", (object)reason ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@Id", id);
            connection.Open();
            cmd.ExecuteNonQuery();
        }

        // --- MAPEAMENTO ---

        private Recipe MapFromReader(SqlDataReader reader)
        {
            var recipe = new Recipe
            {
                RecipeId = Convert.ToInt64(reader["RecipeId"]),
                Title = reader["Title"]?.ToString() ?? "",
                PreparationMethod = reader["PreparationMethod"]?.ToString() ?? "",
                PreparationTime = Convert.ToInt32(reader["PreparationTime"]),
                CategoryId = Convert.ToInt32(reader["CategoryId"]),
                DifficultyId = Convert.ToInt32(reader["DifficultyId"]),
                CreatedByUserId = Convert.ToInt64(reader["CreatedByUserId"]),
                CategoryName = reader["CategoryName"]?.ToString() ?? "",
                DifficultyName = reader["DifficultyName"]?.ToString() ?? "",
                IsApproved = reader["IsApproved"] != DBNull.Value && Convert.ToBoolean(reader["IsApproved"]),
                CreatedAt = Convert.ToDateTime(reader["CreatedAt"]),
                AverageRating = reader.HasColumn("AverageRating") ? Convert.ToDouble(reader["AverageRating"]) : 0,
                IsFavorite = reader.HasColumn("IsFavorite") && Convert.ToInt32(reader["IsFavorite"]) == 1,
                IngredientsCount = reader.HasColumn("IngredientsCount") ? Convert.ToInt32(reader["IngredientsCount"]) : 0
            };

            // Lê o RejectionReason se a coluna existir no SELECT
            if (reader.HasColumn("RejectionReason"))
            {
                recipe.RejectionReason = reader["RejectionReason"]?.ToString();
            }

            return recipe;
        }
    }
}

public static class SqlReaderExtensions
{
    public static bool HasColumn(this SqlDataReader reader, string columnName)
    {
        for (int i = 0; i < reader.FieldCount; i++)
        {
            if (reader.GetName(i).Equals(columnName, StringComparison.OrdinalIgnoreCase))
                return true;
        }
        return false;
    }
}