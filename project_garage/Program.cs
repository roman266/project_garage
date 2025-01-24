using Microsoft.EntityFrameworkCore;
using project_garage.Data;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using project_garage.Models.DbModels;
using project_garage.Interfaces.IRepository;
using project_garage.Repository;
using project_garage.Interfaces.IService;
using project_garage.Service;

var builder = WebApplication.CreateBuilder(args);

// Налаштування служб
builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    // Логування запитів до бази для відлагодження
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection"))
           .EnableSensitiveDataLogging(); // Для відлагодження
});

builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IAuthRepository, AuthRepository>();
builder.Services.AddScoped<IPostRepository, PostRepository>();
builder.Services.AddScoped<IFriendRepository, FriendRepository>();
builder.Services.AddScoped<IPostService, PostService>();
builder.Services.AddScoped<IFriendService, FriendService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IConversationRepository, ConversationRepository>();
builder.Services.AddScoped<IMessageRepository, MessageRepository>();
builder.Services.AddScoped<IConversationService, ConversationService>();
builder.Services.AddScoped<IMessageService, MessageService>();

builder.Services.AddIdentity<UserModel, IdentityRole>(options =>
{
    options.Password.RequiredLength = 8;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireDigit = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireNonAlphanumeric= false;
    options.User.RequireUniqueEmail = true;
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();

builder.Services.AddTransient<IEmailSender, EmailSender>();

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

app.MapControllerRoute(
    name: "profile",
    pattern: "User/Profile/{userId}",
    defaults: new { controller = "Profile", action = "ProfileIndex" });

app.MapControllerRoute(
    name: "profile-search",
    pattern: "Profile/SearchUsers",
    defaults: new { controller = "ProfileController", action = "SearchUsers" });


app.Run();
