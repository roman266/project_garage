
namespace project_garage.Models.DbModels
{
    public class UserConversationModel
    {
        public string Id { get; set; }

        public string UserId { get; set; }
        public UserModel User { get; set; } = null!;

        public string ConversationId { get; set; }
        public ConversationModel Conversation { get; set; } = null!;

        public DateTime JoinedAt { get; set; } = DateTime.UtcNow;
    }

}
