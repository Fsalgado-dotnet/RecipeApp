using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RecipeApp.Models;
using RecipeApp.Services; //  SessionHelper e RecipeService

namespace RecipeApp.Web.Pages.Admin
{
    public class ApproveRecipesModel : PageModel
    {
        private readonly RecipeService _recipeService;

        // Injetamos o Service aqui
        public ApproveRecipesModel(RecipeService recipeService)
        {
            _recipeService = recipeService;
        }

        public List<Recipe> PendingRecipes { get; set; } = new();

        public IActionResult OnGet()
        {
            // Validação de Admin
            if (!SessionHelper.IsAdmin(HttpContext))
                return RedirectToPage("/Index");

            // Usamos o Service agora
            PendingRecipes = _recipeService.GetPendingRecipes();
            return Page();
        }

        public IActionResult OnPostApprove(long recipeId)
        {
            if (!SessionHelper.IsAdmin(HttpContext))
                return RedirectToPage("/Index");

            _recipeService.ApproveRecipe(recipeId);
            TempData["SuccessMessage"] = "Receita aprovada com sucesso!";
            return RedirectToPage();
        }

        // --- NOVO MÉTODO PARA REJEITAR COM MOTIVO ---
        public IActionResult OnPostReject(long recipeId, string reason)
        {
            // 1. Validar se é Admin
            if (!SessionHelper.IsAdmin(HttpContext))
                return RedirectToPage("/Index");

            // 2. Validar se o ID é válido
            if (recipeId <= 0)
                return RedirectToPage();

            // 3. Definir motivo padrão caso o Admin não escreva nada
            string finalReason = string.IsNullOrWhiteSpace(reason)
                                 ? "A receita não cumpre os requisitos mínimos de qualidade da plataforma."
                                 : reason;

            // 4. Chamar o Service (o método que criámos anteriormente na DAL e Service)
            _recipeService.RejectRecipe(recipeId, finalReason);

            // 5. Feedback visual
            TempData["SuccessMessage"] = "Receita reprovada com sucesso!";

            return RedirectToPage();
        }
    }
}