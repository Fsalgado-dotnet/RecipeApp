using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RecipeApp.Web.DAL;
using RecipeApp.Web.Models;
using RecipeApp.Web.Services;
using System;

namespace RecipeApp.Web.Pages
{
    public class RegisterModel : PageModel
    {
        private readonly UserDAL _userDAL;

        public RegisterModel(UserDAL userDAL)
        {
            _userDAL = userDAL;
        }

        [BindProperty]
        // O 'new' resolve o conflito com PageModel.User (CS0108)
        // O '= new();' resolve o aviso de inicialização (CS8618)
        public new User User { get; set; } = new();

        public void OnGet()
        {
        }

        public IActionResult OnPost()
        {
            // Validar se o formulário foi preenchido corretamente segundo o Modelo
            if (!ModelState.IsValid)
            {
                return Page();
            }

            // 1. Verificar email duplicado (usando o email que veio do formulário)
            if (_userDAL.EmailExists(User.Email))
            {
                ModelState.AddModelError("User.Email", "Já existe um utilizador com este email.");
                return Page();
            }

            try
            {
                // 2. Preparar dados
                // Assumindo que o campo da password no formulário mapeia para PasswordHash temporariamente
                User.PasswordHash = PasswordHelper.HashPassword(User.PasswordHash);
                User.IsAdmin = false;
                User.IsLocked = false;
                User.CreatedAt = DateTime.Now;

                // 3. Guardar na Base de Dados
                _userDAL.CreateUser(User);

                // 4. Redirecionar para o Login com sucesso
                return RedirectToPage("/Login");
            }
            catch (Exception ex)
            {
                // Grava o erro detalhado no Output do Visual Studio para tu veres enquanto programas
                System.Diagnostics.Debug.WriteLine($"Erro Crítico no Registo: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"StackTrace: {ex.StackTrace}");

                ModelState.AddModelError("", "Ocorreu um erro ao registar o utilizador. Tente novamente.");
                return Page();
            }
        }
    }
}