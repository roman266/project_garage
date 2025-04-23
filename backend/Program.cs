using DotNetEnv;
using Microsoft.EntityFrameworkCore;
using project_garage.Bogus;
using project_garage.Data;
using project_garage.Extensions;
using project_garage.Middleware;
using project_garage.Service;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;

Env.Load();

builder.Services.AddHttpsRedirection(options =>
{
    options.HttpsPort = 7126;
});

builder.Services.AddApplicationServices(configuration);
builder.Services.AddIdentity();
builder.Services.ConfigureApplicationCookies();
builder.Services.AddController();
builder.Services.AddAuthorization(configuration);
builder.Services.AddSignalR();
builder.Services.AddCors(configuration);

var app = builder.Build();
/*
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var seedService = services.GetRequiredService<SeedService>();
        await seedService.SeedDatabaseAsync(true); 
        app.Logger.LogInformation("Database seeding completed successfully.");
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred during database seeding.");
    }
}
*/

app.UseMiddleware<EnsureInterestsMiddleware>();
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseCors("AllowFrontend");
app.UseAuthentication();
app.UseAuthorization();
app.UseWebSockets();
app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
    endpoints.MapHub<ChatHub>("/chatHub").RequireAuthorization();
});

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
Console.WriteLine($"Connection String: {connectionString}");

app.MapControllers();


//uncomment if you need to populate ConversationModel, UserConversationModel and AspNetUsers with data
//using (var scope = app.Services.CreateScope())
//{
//   var dataSeeder = scope.ServiceProvider.GetRequiredService<DataSeeder>();
//   await dataSeeder.SeedAsync();
//}

app.Run();