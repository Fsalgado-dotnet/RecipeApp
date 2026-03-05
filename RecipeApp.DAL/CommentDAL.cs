using Microsoft.Data.SqlClient;
using RecipeApp.Models;
using System.Collections.Generic;

namespace RecipeApp.DAL
{
    public class CommentDAL
    {
        private readonly DbHelper _db;

        public CommentDAL(DbHelper db)
        {
            _db = db;
        }

        public List<Comment> GetByRecipe(long recipeId)
        {
            var comments = new List<Comment>();
            try
            {
                using var connection = _db.GetConnection();
                string sql = @"
                    SELECT c.CommentId, c.Text, c.CreatedAt, u.Name AS UserName
                    FROM Comment c
                    INNER JOIN Users u ON c.UserId = u.UserId
                    WHERE c.RecipeId = @RecipeId
                    ORDER BY c.CreatedAt DESC";

                using var cmd = new SqlCommand(sql, connection);
                cmd.Parameters.AddWithValue("@RecipeId", recipeId);

                connection.Open();
                using var reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    comments.Add(new Comment
                    {
                        CommentId = Convert.ToInt64(reader["CommentId"]),
                        Text = reader["Text"]?.ToString() ?? "",
                        UserName = reader["UserName"]?.ToString() ?? "Anónimo",
                        CreatedAt = Convert.ToDateTime(reader["CreatedAt"])
                    });
                }
            }
            catch { /* falha silenciosa para não quebrar a página de detalhes */ }
            return comments;
        }

        public void Create(Comment comment)
        {
            using var connection = _db.GetConnection();
            string sql = "INSERT INTO Comment (RecipeId, UserId, Text, CreatedAt) VALUES (@RecipeId, @UserId, @Text, @CreatedAt)";

            using var cmd = new SqlCommand(sql, connection);
            cmd.Parameters.AddWithValue("@RecipeId", comment.RecipeId);
            cmd.Parameters.AddWithValue("@UserId", comment.UserId);
            cmd.Parameters.AddWithValue("@Text", comment.Text);
            cmd.Parameters.AddWithValue("@CreatedAt", comment.CreatedAt);

            connection.Open();
            cmd.ExecuteNonQuery();
        }
    }
}