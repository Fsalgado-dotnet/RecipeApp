using RecipeApp.DAL;
using RecipeApp.Services;
using RecipeApp.Models;

var builder = WebApplication.CreateBuilder(args);

// ===============================
// SERVICES (Configuração)
// ===============================
builder.Services.AddRazorPages();

// DB helper
builder.Services.AddScoped<DbHelper>();

// DALs (Acesso Direto ao SQL)
builder.Services.AddScoped<UserDAL>();
builder.Services.AddScoped<RecipeDAL>();
builder.Services.AddScoped<CategoryDAL>();
builder.Services.AddScoped<DifficultyDAL>();
builder.Services.AddScoped<IngredientDAL>();
builder.Services.AddScoped<RecipeIngredientDAL>();
builder.Services.AddScoped<CommentDAL>();
builder.Services.AddScoped<RatingDAL>();
builder.Services.AddScoped<FavoriteDAL>();

// SERVICES (Lógica de Negócio) 
builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<RecipeService>();
builder.Services.AddScoped<CategoryService>();
builder.Services.AddScoped<DifficultyService>();
builder.Services.AddScoped<IngredientService>();
builder.Services.AddScoped<RecipeIngredientService>();
builder.Services.AddScoped<CommentService>();
builder.Services.AddScoped<RatingService>();
builder.Services.AddScoped<FavoriteService>();

// Session
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

var app = builder.Build();

// ===============================
// MIDDLEWARE (Pipeline)
// ===============================
if (!app.Environment.IsDevelopment())
{
    // erro + protocolo de segurança via HTTPS
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}
// mudança forcada para HTTPS + Permitir ler ficheiros css do wwwroot
app.UseHttpsRedirection();
app.UseStaticFiles();
// para decidir que pagina correr quando o user escrevre no URL
app.UseRouting();
// sesssionhelper - para lembrar quem é o utilizador a cada mudanca de pagina e cliques
app.UseSession();
// verifica se o user tem autorizacao para visitar a pagina pedida
app.UseAuthorization();
// configurar os endpoints das paginas .cshtml
app.MapRazorPages();

app.Run();