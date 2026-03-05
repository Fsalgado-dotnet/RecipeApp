using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace RecipeApp.Web.Pages
{
    // ResponseCache impede que o browser guarde a página de erro em cache
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    [IgnoreAntiforgeryToken]
    public class ErrorModel : PageModel
    {
        public string? RequestId { get; set; }
        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);

        private readonly ILogger<ErrorModel> _logger;

        public ErrorModel(ILogger<ErrorModel> logger)
        {
            _logger = logger;
        }

        public void OnGet()
        {
            // Captura o ID da operação para debug
            RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier;

            // Logar o erro internamente para o administrador
            _logger.LogError($"Erro apresentado ao utilizador. Request ID: {RequestId}");
        }
    }
}