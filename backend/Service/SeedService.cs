using Bogus;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using project_garage.Data;
using project_garage.Models.DbModels;
using project_garage.Models.Enums;
using Microsoft.AspNetCore.Identity;

namespace project_garage.Service  
{
    public class SeedService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<SeedService> _logger;
        private readonly IPasswordHasher<UserModel> _passwordHasher;

        public SeedService(ApplicationDbContext context, ILogger<SeedService> logger, IPasswordHasher<UserModel> passwordHasher)
        {
            _context = context;
            _logger = logger;
            _passwordHasher = passwordHasher;
        }

        public async Task SeedDatabaseAsync(bool force = false)
        {
            var userCount = await _context.Users.CountAsync();
            _logger.LogInformation("Current user count: {UserCount}", userCount);

            if (!force && userCount > 0)
            {
                _logger.LogInformation("Database already seeded. Skipping...");
                return;
            }

            _logger.LogInformation("Starting database seeding...");
            var random = new Random();
            var interestTypes = _context.Interests.Select(i => i.Id).ToList();
            //var userIds = _context.Users.Select(u => u.Id).ToList();

            try
            {
                // 1. Додаємо тестового користувача
                UserModel testUser = await _context.Users.FirstOrDefaultAsync(u => u.UserName == "testuser");
                if (testUser == null)
                {
                    testUser = new UserModel
                    {
                        Id = Guid.NewGuid().ToString(),
                        UserName = "testuser",
                        NormalizedUserName = "TESTUSER",
                        Email = "testuser@example.com",
                        NormalizedEmail = "TESTUSER@EXAMPLE.COM",
                        EmailConfirmed = true,
                        EmailConfirmationCode = Guid.NewGuid().ToString(),
                        CreatedAt = DateTime.UtcNow
                    };
                    testUser.PasswordHash = _passwordHasher.HashPassword(testUser, "Test@1234");
                    await _context.Users.AddAsync(testUser);
                    await _context.SaveChangesAsync();
                    _logger.LogInformation("✅ Added test user: testuser");
                }
                else
                {
                    _logger.LogInformation("Test user 'testuser' already exists. Skipping...");
                }

                // 2. Створюємо випадкових користувачів
                var userFaker = new Faker<UserModel>()
                    .RuleFor(u => u.Id, f => Guid.NewGuid().ToString())
                    .RuleFor(u => u.FirstName, f => f.Name.FirstName())
                    .RuleFor(u => u.LastName, f => f.Name.LastName())
                    .RuleFor(u => u.UserName, (f, u) => $"{u.FirstName.ToLower()}.{u.LastName.ToLower()}{f.Random.Number(1000, 9999)}")
                    .RuleFor(u => u.NormalizedUserName, (f, u) => u.UserName.ToUpper())
                    .RuleFor(u => u.Email, (f, u) => $"{u.FirstName.ToLower()}.{u.LastName.ToLower()}{f.Random.Number(1000, 9999)}@example.com")
                    .RuleFor(u => u.NormalizedEmail, (f, u) => u.Email.ToUpper())
                    .RuleFor(u => u.EmailConfirmed, f => true)
                    .RuleFor(u => u.EmailConfirmationCode, f => Guid.NewGuid().ToString())
                    .RuleFor(u => u.PasswordHash, f => _passwordHasher.HashPassword(null, f.Internet.Password()))
                    .RuleFor(u => u.CreatedAt, f => f.Date.Past(3));

                var users = userFaker.Generate(100);
                await _context.Users.AddRangeAsync(users);
                await _context.SaveChangesAsync();
                _logger.LogInformation("✅ Added 50 random users.");

                // Додаємо інтереси користувачам
                var allUsers = await _context.Users.ToListAsync();
                var interests = new List<UserInterestModel>();
                foreach (var user in allUsers)
                {
                    int interestCount = random.Next(2, 5);
                    var userInterests = Enumerable.Range(0, interestCount)
                        .Select(_ => new UserInterestModel
                        {
                            Id = Guid.NewGuid().ToString(),
                            UserId = user.Id,
                            InterestId = interestTypes[random.Next(interestTypes.Count)],
                        })
                        .ToList();
                    interests.AddRange(userInterests);
                }
                await _context.UserInterests.AddRangeAsync(interests);
                await _context.SaveChangesAsync();
                _logger.LogInformation("✅ Added {Count} user interests.", interests.Count);

                // Додаємо друзів для всіх користувачів
                var friends = new HashSet<(string, string)>();
                foreach (var user in allUsers)
                {
                    var possibleFriends = allUsers
                        .Where(u => u.Id != user.Id)
                        .OrderBy(_ => random.Next())
                        .Take(random.Next(3, 10))
                        .ToList();

                    foreach (var friend in possibleFriends)
                    {
                        if (!friends.Contains((user.Id, friend.Id)) && !friends.Contains((friend.Id, user.Id)))
                        {
                            friends.Add((user.Id, friend.Id));
                        }
                    }
                }
                var friendModels = friends.Select(f => new FriendModel
                {
                    Id = Guid.NewGuid().ToString(),
                    UserId = f.Item1,
                    FriendId = f.Item2,
                    IsAccepted = true
                }).ToList();

                await _context.Friends.AddRangeAsync(friendModels);
                await _context.SaveChangesAsync();
                _logger.LogInformation("✅ Added {Count} friend relationships.", friendModels.Count);

                // Додаємо пости
                var postFaker = new Faker<PostModel>()
                    .RuleFor(p => p.Description, f => f.Lorem.Sentence(10))
                    .RuleFor(p => p.CreatedAt, f => f.Date.Past(2))
                    .RuleFor(p => p.InterestId, f => f.PickRandom(interestTypes))
                    .RuleFor(p => p.UpdatedAt, f => f.Date.Past(2))
                    .RuleFor(p => p.ImageUrl, f => f.Internet.Url());


                var posts = new List<PostModel>();
                foreach (var user in allUsers)
                {
                    int postCount = random.Next(5, 10);
                    var userPosts = postFaker.Generate(postCount);
                    userPosts.ForEach(p => p.UserId = user.Id);
                    posts.AddRange(userPosts);
                }
                await _context.Posts.AddRangeAsync(posts);
                await _context.SaveChangesAsync();
                _logger.LogInformation("✅ Added {Count} posts.", posts.Count);

                _logger.LogInformation("Database seeding completed successfully!");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred during database seeding.");
                throw;
            }
        }
    }
}
