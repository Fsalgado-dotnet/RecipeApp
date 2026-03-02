using Microsoft.Data.SqlClient;
using RecipeApp.Web.Models;

namespace RecipeApp.Web.DAL
{
    public class UserDAL
    {
        private readonly DbHelper _db;

        public UserDAL(DbHelper db)
        {
            _db = db;
        }

        // 🔹 Criar utilizador (Register)
        public void CreateUser(User user)
        {
            using var connection = _db.GetConnection();

            string sql = @"
                INSERT INTO Users (Name, Email, PasswordHash, IsAdmin, IsLocked, CreatedAt)
                VALUES (@Name, @Email, @PasswordHash, @IsAdmin, @IsLocked, @CreatedAt)
            ";

            using var cmd = new SqlCommand(sql, connection);

            cmd.Parameters.AddWithValue("@Name", user.Name);
            cmd.Parameters.AddWithValue("@Email", user.Email);
            cmd.Parameters.AddWithValue("@PasswordHash", user.PasswordHash);
            cmd.Parameters.AddWithValue("@IsAdmin", user.IsAdmin);
            cmd.Parameters.AddWithValue("@IsLocked", user.IsLocked);
            cmd.Parameters.AddWithValue("@CreatedAt", user.CreatedAt);

            connection.Open();
            cmd.ExecuteNonQuery();
        }

        // 🔹 Verificar se email já existe (Register)
        public bool EmailExists(string email)
        {
            using var connection = _db.GetConnection();

            string sql = "SELECT COUNT(1) FROM Users WHERE Email = @Email";

            using var cmd = new SqlCommand(sql, connection);
            cmd.Parameters.AddWithValue("@Email", email);

            connection.Open();
            int count = (int)cmd.ExecuteScalar();

            return count > 0;
        }

        // 🔹 Obter utilizador pelo email (Login)
        public User? GetByEmail(string email)
        {
            using var connection = _db.GetConnection();

            string sql = "SELECT * FROM Users WHERE Email = @Email";

            using var cmd = new SqlCommand(sql, connection);
            cmd.Parameters.AddWithValue("@Email", email);

            connection.Open();

            using var reader = cmd.ExecuteReader();

            if (!reader.Read())
                return null;

            return new User
            {
                UserId = (long)reader["UserId"],
                Name = reader["Name"].ToString()!,
                Email = reader["Email"].ToString()!,
                PasswordHash = reader["PasswordHash"].ToString()!,
                IsAdmin = (bool)reader["IsAdmin"],
                IsLocked = (bool)reader["IsLocked"],
                CreatedAt = (DateTime)reader["CreatedAt"]
            };
        }
    }
}
