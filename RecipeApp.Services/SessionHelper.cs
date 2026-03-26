using RecipeApp.Models;
using System.Text.Json;
using Microsoft.AspNetCore.Http;

namespace RecipeApp.Services
{
    public static class SessionHelper
    {
        // 1. Gravar o utilizador na sessão 
        public static void SetUser(HttpContext context, User user)
        {
            var data = JsonSerializer.Serialize(user);  // Uso Serialize para converter o utilizador em texto e guardá-lo com SetString
            context.Session.SetString("User", data);  // guarda dados na sessão
        }

        // 2. Limpar a sessão 
        public static void Logout(HttpContext context)
        {
            context.Session.Clear();
        }

        public static User? GetUser(HttpContext context)
        {
            var data = context.Session.GetString("User");  // le os dados do user na sessão atual.
            return data == null ? null : JsonSerializer.Deserialize<User>(data);  // transformar de volta num objeto User, para poder verificar userID e Admin
        }

        public static bool IsLoggedIn(HttpContext context)
        {
            return GetUser(context) != null;
        }

        public static bool IsAdmin(HttpContext context)
        {
            var user = GetUser(context);
            return user != null && user.IsAdmin;
        }
    }
}