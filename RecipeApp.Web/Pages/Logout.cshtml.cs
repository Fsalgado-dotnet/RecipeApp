using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RecipeApp.Services;

namespace RecipeApp.Web.Pages
{
    public class LogoutModel : PageModel
    {
        public IActionResult OnGet()
        {
            // 1. Usar o Helper para garantir que a sessão é limpa corretamente
            SessionHelper.Logout(HttpContext);

            // 2. Feedback visual para o utilizador saber que saiu com sucesso
            TempData["SuccessMessage"] = "Sessão terminada. Até à próxima!";

            // 3. Redirecionar para o Login ou Index
            return RedirectToPage("/Login");
        }
    }
}