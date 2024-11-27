using Microsoft.EntityFrameworkCore;
using project_garage.Data;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using project_garage.Models;

var builder = WebApplication.CreateBuilder(args);

// Налаштування служб
builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    // Логування запитів до бази для відлагодження
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection"))
           .EnableSensitiveDataLogging(); // Для відлагодження
});

builder.Services.AddIdentity<UserModel, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

builder.Services.AddControllersWithViews();

// Налаштування автентифікації
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";
        options.AccessDeniedPath = "/Account/AccessDenied";
    });

// Додаткові налаштування авторизації (можна кастомізувати)
builder.Services.AddAuthorization();

var app = builder.Build();

// Налаштування HTTP-конвеєра
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

// HTTPS і статика
app.UseHttpsRedirection();
app.UseStaticFiles();

// Маршрутизація
app.UseRouting();

// Підключення автентифікації та авторизації
app.UseAuthentication();
app.UseAuthorization();

// Перевірка рядка підключення під час запуску
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
Console.WriteLine($"Connection String: {connectionString}");

// Налаштування маршрутів
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
