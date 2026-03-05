using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RecipeApp.Models;
using RecipeApp.Services;

namespace RecipeApp.Web.Pages
{
    public class RegisterModel : PageModel
    {
        private readonly UserService _userService;

        public RegisterModel(UserService userService)
        {
            _userService = userService;
        }

        [BindProperty]
        public new User User { get; set; } = new();

        // Propriedade extra para confirmação de password (boa prática de UX)
        [BindProperty]
        public string ConfirmPassword { get; set; }= string.Empty;

        public void OnGet()
        {
        }

        public IActionResult OnPost()
        {
            // 1. Validação básica do ModelState
            if (!ModelState.IsValid)
            {
                return Page();
            }

            // 2. Verificação de passwords iguais
            if (User.PasswordHash != ConfirmPassword)
            {
                ModelState.AddModelError("ConfirmPassword", "As passwords não coincidem.");
                return Page();
            }

            // 3. Tentar registar via Serviço
            try
            {
                // O serviço agora trata do Hash, CreatedAt e IsAdmin automaticamente
                _userService.Register(User.Name, User.Email, User.PasswordHash);

                TempData["SuccessMessage"] = "Conta criada com sucesso! Já podes fazer login.";
                return RedirectToPage("/Login");
            }
            catch (ArgumentException ex)
            {
                // Erros de negócio (ex: email já existe)
                ModelState.AddModelError("User.Email", ex.Message);
                return Page();
            }
            catch (Exception ex)
            {
                // Erros técnicos inesperados
                System.Diagnostics.Debug.WriteLine($"Erro no Registo: {ex.Message}");
                ModelState.AddModelError("", "Ocorreu um erro inesperado. Tente mais tarde.");
                return Page();
            }
        }
    }
}