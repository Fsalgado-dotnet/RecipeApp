using RecipeApp.Web.Models;
using System.Text.Json;

namespace RecipeApp.Web.Services
{
    public static class SessionHelper
    {
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
