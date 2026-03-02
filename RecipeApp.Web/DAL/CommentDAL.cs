using Microsoft.Data.SqlClient;
using RecipeApp.Web.Models;
using System.Collections.Generic;
using System;

namespace RecipeApp.Web.DAL
{
    public class CommentDAL
    {
        private readonly DbHelper _db;

        public CommentDAL(DbHelper db)
        {
            _db = db;
        }

        // 🔹 Obter comentários de uma receita
        public List<Comment> GetByRecipe(long recipeId)
        {
            var comments = new List<Comment>();

            using var connection = _db.GetConnection();

            string sql = @"
                SELECT 
                    c.CommentId,
                    c.Text,
                    c.CreatedAt,
                    u.Name AS UserName
                FROM Comment c
                INNER JOIN Users u ON c.UserId = u.UserId
                WHERE c.RecipeId = @RecipeId
                ORDER BY c.CreatedAt DESC
            ";

            using var cmd = new SqlCommand(sql, connection);
            cmd.Parameters.AddWithValue("@RecipeId", recipeId);

            connection.Open();
            using var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                comments.Add(new Comment
                {
                    // A correção mágica está aqui: Convert.ToInt64
                    CommentId = Convert.ToInt64(reader["CommentId"]),
                    Text = reader["Text"]?.ToString() ?? "",
                    UserName = reader["UserName"]?.ToString() ?? "Anónimo",
                    CreatedAt = Convert.ToDateTime(reader["CreatedAt"])
                });
            }

            return comments;
        }

        // 🔹 Criar comentário
        public void Create(Comment comment)
        {
            using var connection = _db.GetConnection();

            string sql = @"
                INSERT INTO Comment (RecipeId, UserId, Text, CreatedAt)
                VALUES (@RecipeId, @UserId, @Text, @CreatedAt)
            ";

            using var cmd = new SqlCommand(sql, connection);

            cmd.Parameters.AddWithValue("@RecipeId", comment.RecipeId);
            cmd.Parameters.AddWithValue("@UserId", comment.UserId);
            cmd.Parameters.AddWithValue("@Text", comment.Text);
            cmd.Parameters.AddWithValue("@CreatedAt", comment.CreatedAt == default ? DateTime.Now : comment.CreatedAt);

            connection.Open();
            cmd.ExecuteNonQuery();
        }
    }
}