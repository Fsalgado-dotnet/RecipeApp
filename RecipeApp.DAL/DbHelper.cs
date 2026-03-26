using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace RecipeApp.DAL
{
    public class DbHelper
    {
        private readonly string _connectionString;

        public DbHelper(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
            // permite que o código vá ler o ficheiro appsettings.json
        }

        public SqlConnection GetConnection()
        {
            return new SqlConnection(_connectionString);
            // Sempre que uma DAL precisa de falar com o SQL, ela pede ao DbHelper uma nova conexão
        }
    }
}
