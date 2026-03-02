using Microsoft.Data.SqlClient;
using RecipeApp.Web.Models;

namespace RecipeApp.Web.DAL
{
    public class IngredientDAL
    {
        private readonly DbHelper _db;

        public IngredientDAL(DbHelper db)
        {
            _db = db;
        }

        // 📋 Listar ingredientes
        public List<Ingredient> GetAll()
        {
            var list = new List<Ingredient>();

            using var connection = _db.GetConnection();

            string sql = @"
                SELECT IngredientId, Name
                FROM Ingredient
                ORDER BY Name
            ";

            using var cmd = new SqlCommand(sql, connection);

            connection.Open();
            using var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                list.Add(new Ingredient
                {
                    IngredientId = (long)reader["IngredientId"],
                    Name = reader["Name"].ToString()
                });
            }

            return list;
        }
    }
}

