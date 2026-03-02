using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RecipeApp.Web.DAL;
using RecipeApp.Web.Services;
using System.Text.Json;

namespace RecipeApp.Web.Pages
{
    public class LoginModel : PageModel
    {
        private readonly UserDAL _userDAL;

        public LoginModel(UserDAL userDAL)
        {
            _userDAL = userDAL;
        }

        [BindProperty]
        public string Email { get; set; }

        [BindProperty]
        public string Password { get; set; }

        public IActionResult OnPost()
        {
            var user = _userDAL.GetByEmail(Email);

            if (user == null)
            {
                ModelState.AddModelError("", "Email ou password inválidos.");
                return Page();
            }

            if (!PasswordHelper.VerifyPassword(Password, user.PasswordHash))
            {
                ModelState.AddModelError("", "Email ou password inválidos.");
                return Page();
            }

            if (user.IsLocked)
            {
                ModelState.AddModelError("", "Conta bloqueada.");
                return Page();
            }

            HttpContext.Session.SetString(
                "User",
                JsonSerializer.Serialize(user)
            );

            return RedirectToPage("/Index");
        }
    }
}

