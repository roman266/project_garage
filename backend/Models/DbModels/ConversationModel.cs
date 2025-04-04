namespace project_garage.Models.DbModels
{
    public class ConversationModel
    {
        public string Id { get; set; }
        public bool IsPrivate { get; set; }
        public DateTime StartedAt { get; set; }
        public DateTime LastUpdatedAt { get; set; }

        public ICollection<MessageModel> Messages { get; set; }
        public ICollection<UserConversationModel> UserConversations { get; set; }
    }
}
