using Microsoft.Data.SqlClient;
using RecipeApp.Web.Models;

namespace RecipeApp.Web.DAL
{
    public class CategoryDAL
    {
        private readonly DbHelper _db;

        public CategoryDAL(DbHelper db)
        {
            _db = db;
        }

        // ===============================
        // LISTAR TODAS AS CATEGORIAS
        // ===============================
        public List<Category> GetAll()
        {
            var list = new List<Category>();

            using var connection = _db.GetConnection();

            string sql = @"
                SELECT CategoryId, Name
                FROM Category
                ORDER BY Name
            ";

            using var cmd = new SqlCommand(sql, connection);

            connection.Open();
            using var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                list.Add(new Category
                {
                    CategoryId = (long)reader["CategoryId"],
                    Name = reader["Name"].ToString()
                });
            }

            return list;
        }
    }
}
