using RecipeApp.Web.Models;
using System.Text.Json;

namespace RecipeApp.Web.Services
{
    public static class SessionHelper
    {
        // 1. Gravar o utilizador na sessão (Resolve erro no Login.cshtml.cs)
        public static void SetUser(HttpContext context, User user)
        {
            var data = JsonSerializer.Serialize(user);
            context.Session.SetString("User", data);
        }

        // 2. Limpar a sessão (Resolve erro no Logout.cshtml.cs)
        public static void Logout(HttpContext context)
        {
            context.Session.Clear();
        }

        public static User? GetUser(HttpContext context)
        {
            var data = context.Session.GetString("User");
            return data == null ? null : JsonSerializer.Deserialize<User>(data);
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