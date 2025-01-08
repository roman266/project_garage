using Microsoft.EntityFrameworkCore;
using project_garage.Data;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using project_garage.Models;

var builder = WebApplication.CreateBuilder(args);

// ������������ �����
builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    // ��������� ������ �� ���� ��� ������������
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection"))
           .EnableSensitiveDataLogging(); // ��� ������������
});

builder.Services.AddIdentity<UserModel, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

builder.Services.AddControllersWithViews();

// ������������ ��������������
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";
        options.AccessDeniedPath = "/Account/AccessDenied";
    });

// �������� ������������ ����������� (����� ������������)
builder.Services.AddAuthorization();

var app = builder.Build();

// ������������ HTTP-�������
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

// HTTPS � �������
app.UseHttpsRedirection();
app.UseStaticFiles();

// �������������
app.UseRouting();

// ϳ��������� �������������� �� �����������
app.UseAuthentication();
app.UseAuthorization();

// �������� ����� ���������� �� ��� �������
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
Console.WriteLine($"Connection String: {connectionString}");

// ������������ ��������
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
