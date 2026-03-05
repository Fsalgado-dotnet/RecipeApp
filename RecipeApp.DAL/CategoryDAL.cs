using Microsoft.Data.SqlClient;
using RecipeApp.Models;
using System.Collections.Generic;

namespace RecipeApp.DAL
{
    public class CategoryDAL
    {
        private readonly DbHelper _db;

        public CategoryDAL(DbHelper db)
        {
            _db = db;
        }

        public List<Category> GetAll()
        {
            var list = new List<Category>();

            try
            {
                using var connection = _db.GetConnection();
                string sql = "SELECT CategoryId, Name FROM Category ORDER BY Name";

                using var cmd = new SqlCommand(sql, connection);
                connection.Open();
                using var reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    list.Add(new Category
                    {
                        CategoryId = Convert.ToInt64(reader["CategoryId"]),
                        Name = reader["Name"]?.ToString() ?? "Sem Categoria"
                    });
                }
            }
            catch
            {
                //  Retorna lista vazia se a BD falhar
            }

            return list;
        }
    }
}