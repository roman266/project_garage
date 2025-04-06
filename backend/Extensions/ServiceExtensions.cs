using Microsoft.EntityFrameworkCore;
using project_garage.Data;
using project_garage.Interfaces.IRepository;
using project_garage.Interfaces.IService;
using project_garage.Models.CloudinarySettings;
using project_garage.Models.JWTSettings;
using project_garage.Repository;
using project_garage.Service;
using Microsoft.AspNetCore.Identity.UI.Services;
using System.Text.Json;
using Microsoft.AspNetCore.Identity;
using project_garage.Models.DbModels;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using project_garage.Bogus;

namespace project_garage.Extensions
{
    public static class ServiceExtensions
    {
        public static void AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseSqlite(configuration.GetConnectionString("DefaultConnection"))
                       .EnableSensitiveDataLogging();
            });

            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IPostRepository, PostRepository>();
            services.AddScoped<ICommentRepository, CommentRepository>();
            services.AddScoped<IFriendRepository, FriendRepository>();
            services.AddScoped<IPostService, PostService>();
            services.AddScoped<ICommentService, CommentService>();
            services.AddScoped<IFriendService, FriendService>();
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IConversationRepository, ConversationRepository>();
            services.AddScoped<IConversationService, ConversationService>();
            services.AddScoped<IMessageRepository, MessageRepository>();
            services.AddScoped<IMessageService, MessageService>();
            services.AddScoped<IReactionRepository, ReactionRepository>();
            services.AddScoped<IReactionService, ReactionService>();
            services.AddScoped<IUserConversationRepository, UserConversationRepository>();
            services.AddScoped<ITokenService, TokenService>();
            services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
            services.AddScoped<IUserInterestRepository, UserInterestRepository>();
            services.AddScoped<IEmailSenderService, EmailSenderService>();
            services.AddScoped<DataSeeder>();
            services.AddScoped<ChatHub>();

            services.Configure<JWTSettings>(configuration.GetSection("Jwt"));
            services.AddSingleton<JWTSettings>();
            services.Configure<CloudinarySettings>(configuration.GetSection("Cloudinary"));
            services.AddSingleton<ICloudinaryService, CloudinaryService>();
        }

        public static void AddIdentity(this IServiceCollection services)
        {
            services.AddIdentity<UserModel, IdentityRole>(options =>
            {
                options.Password.RequiredLength = 8;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireDigit = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.User.RequireUniqueEmail = true;
            })
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();
        }

        public static void ConfigureApplicationCookies(this IServiceCollection services)
        {
            services.ConfigureApplicationCookie(options =>
            {
                options.Cookie.HttpOnly = true;
                options.Cookie.SameSite = SameSiteMode.None;
                options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
                options.Cookie.Expiration = TimeSpan.Zero;
                options.SlidingExpiration = false;
                options.LoginPath = PathString.Empty;
                options.AccessDeniedPath = PathString.Empty;
                options.Events.OnRedirectToLogin = context =>
                {
                    context.Response.StatusCode = 401;
                    return Task.CompletedTask;
                };
            });
        }

        public static void AddController(this IServiceCollection services)
        {
            services.AddControllers().AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.Preserve;
            });
        }

        public static void AddAuthorization(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(options =>
                {
                    options.Events = new JwtBearerEvents
                    {
                        OnMessageReceived = context =>
                            {
                                if (context.Request.Cookies.ContainsKey("AccessToken"))
                                {
                                    context.Token = context.Request.Cookies["AccessToken"];
                                }
                                return Task.CompletedTask;
                            }
                        };

                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ClockSkew = TimeSpan.Zero,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = configuration["Jwt:Issuer"],
                        ValidAudience = configuration["Jwt:Audience"],
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding
                        .UTF8.GetBytes(configuration["Jwt:Key"]))
                    };
                });
        }

        public static void AddCors(this IServiceCollection services, IConfiguration configuration)
        {
            var allowedOrigins = configuration.GetSection("AllowedOrigins").Get<string[]>();

            services.AddCors(options =>
            {
                options.AddPolicy("AllowFrontend",
                    policy =>
                    {
                        policy.WithOrigins(allowedOrigins)
                              .AllowCredentials()
                              .AllowAnyMethod()
                              .AllowAnyHeader();
                    });
            });
        }
    }
}
