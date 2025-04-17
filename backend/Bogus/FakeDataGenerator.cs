using Bogus;
using project_garage.Models.DbModels;
using project_garage.Models.ViewModels;

namespace project_garage.Bogus
{
    public class FakeDataGenerator
    {
        private readonly Faker _faker;

        public FakeDataGenerator()
        {
            _faker = new Faker();
        }

        public List<RegisterDto> GenerateUsers(int count)
        {
            var users = new List<RegisterDto>();

            for (int i = 0; i < count; i++)
            {
                var user = new RegisterDto
                {
                    UserName = _faker.Internet.UserName(),
                    Email = _faker.Internet.Email(),
                    Password = "12345678",
                };

                users.Add(user);
            }

            return users;
        }

        public List<ConversationModel> GenerateConversations(int count)
        {
            var conversations = new List<ConversationModel>();

            for (int i = 0; i < count; i++)
            {
                var conversation = new ConversationModel
                {
                    Id = Guid.NewGuid().ToString(),
                    IsPrivate = true,
                    StartedAt = DateTime.Now,
                };
                conversations.Add(conversation);
            }

            return conversations;
        }

        public List<UserConversationModel> GenerateUserConversationModels(List<string> conversations, List<string> users)
        {
            var userConversations = new List<UserConversationModel>();

            foreach (var conversation in conversations)
            {
                var userConversation = new UserConversationModel
                {
                    Id = Guid.NewGuid().ToString(),
                    ConversationId = conversation,
                    UserId = "bc009f7b-bfff-456a-b5ce-e46aea51d4ee",
                };

                userConversations.Add(userConversation);

                var secondUserId = users.FirstOrDefault();
                var secondConversation = new UserConversationModel
                {
                    Id = Guid.NewGuid().ToString(),
                    ConversationId = conversation,
                    UserId = secondUserId,
                };
                userConversations.Add(secondConversation);
                users.Remove(secondUserId);
            }

            return userConversations;
        }
    }
}
