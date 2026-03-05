using Microsoft.Data.SqlClient;
using RecipeApp.Models;
using System.Collections.Generic;

namespace RecipeApp.DAL
{
    public class IngredientDAL
    {
        private readonly DbHelper _db;

        public IngredientDAL(DbHelper db)
        {
            _db = db;
        }

        public List<Ingredient> GetAll()
        {
            var list = new List<Ingredient>();

            try
            {
                using var connection = _db.GetConnection();
                string sql = "SELECT IngredientId, Name FROM Ingredient ORDER BY Name";

                using var cmd = new SqlCommand(sql, connection);
                connection.Open();
                using var reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    list.Add(new Ingredient
                    {
                        IngredientId = (long)reader["IngredientId"],
                        Name = reader["Name"]?.ToString() ?? "Ingrediente sem nome"
                    });
                }
            }
            catch
            {
                // Retorna lista vazia para evitar crash na UI
            }

            return list;
        }
    }
}