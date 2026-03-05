using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RecipeApp.Services;

namespace RecipeApp.Web.Pages
{
    public class LoginModel : PageModel
    {
        private readonly UserService _userService;

        public LoginModel(UserService userService)
        {
            _userService = userService;
        }

        [BindProperty]
        public string Email { get; set; }

        [BindProperty]
        public string Password { get; set; }

        public void OnGet()
        {
            // Apenas renderiza a página de login
        }

        public IActionResult OnPost()
        {
            // 1. Validação básica de campos vazios
            if (string.IsNullOrEmpty(Email) || string.IsNullOrEmpty(Password))
            {
                ModelState.AddModelError("", "Por favor, preencha todos os campos.");
                return Page();
            }

            // 2. Autenticação via Serviço
            // O serviço trata de: procurar user, verificar hash e checar se está bloqueado
            var user = _userService.Authenticate(Email, Password);

            if (user == null)
            {
                // Mensagem genérica para não dar pistas a hackers
                ModelState.AddModelError("", "Email ou password inválidos.");
                return Page();
            }

            if (user.IsLocked)
            {
                ModelState.AddModelError("", "Esta conta encontra-se bloqueada. Contacte o administrador.");
                return Page();
            }

            // 3. Gravar na Sessão usando o nosso Helper (abstrai o JsonSerializer)
            SessionHelper.SetUser(HttpContext, user);

            // 4. Feedback e Redirecionamento
            TempData["SuccessMessage"] = $"Bem-vindo de volta, {user.Name}!";
            return RedirectToPage("/Index");
        }
    }
}