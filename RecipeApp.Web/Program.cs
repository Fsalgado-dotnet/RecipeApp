using RecipeApp.Web.DAL;
using RecipeApp.Web.Services;

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
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseSession();

app.UseAuthorization();

app.MapRazorPages();

app.Run();