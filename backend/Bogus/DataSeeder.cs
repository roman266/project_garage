using project_garage.Data;
using project_garage.Interfaces.IService;

namespace project_garage.Bogus
{
    public class DataSeeder
    {
        private readonly IUserService _userService;
        private readonly FakeDataGenerator _fakeDataGenerator;
        private readonly ApplicationDbContext _context;

        public DataSeeder(IUserService userService, ApplicationDbContext context)
        {
            _userService = userService;
            _fakeDataGenerator = new FakeDataGenerator();
            _context = context;
        }

        public async Task SeedAsync()
        {
            var users = _fakeDataGenerator.GenerateUsers(0);

            foreach (var user in users)
            {
                await _userService.CreateUserAsync(user);
                var newUser = await _userService.GetByEmailAsync(user.Email);
                await _userService.ConfirmUserEmail(newUser.Id, newUser.EmailConfirmationCode);
            }

            var conversations = _fakeDataGenerator.GenerateConversations(50);
            _context.Conversations.AddRange(conversations);
            await _context.SaveChangesAsync();

            var conversationIds = _context.Conversations.Select(c => c.Id).ToList();
            var userIds = _context.Users.Select(u => u.Id).ToList();
            
            var userConversations = _fakeDataGenerator.GenerateUserConversationModels(conversationIds, userIds);

            _context.UserConversations.AddRange(userConversations);
            await _context.SaveChangesAsync();
        }
    }
}
