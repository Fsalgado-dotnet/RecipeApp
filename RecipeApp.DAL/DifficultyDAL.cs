using Microsoft.Data.SqlClient;
using RecipeApp.Models;
using System.Collections.Generic;

namespace RecipeApp.DAL
{
    public class DifficultyDAL
    {
        private readonly DbHelper _db;

        public DifficultyDAL(DbHelper db)
        {
            _db = db;
        }

        public List<Difficulty> GetAll()
        {
            var list = new List<Difficulty>();

            try
            {
                using var connection = _db.GetConnection();
                string sql = "SELECT DifficultyId, Name FROM Difficulty ORDER BY DifficultyId";

                using var cmd = new SqlCommand(sql, connection);
                connection.Open();
                using var reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    list.Add(new Difficulty
                    {
                        DifficultyId = (long)reader["DifficultyId"],
                        Name = reader["Name"]?.ToString() ?? "N/A"
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