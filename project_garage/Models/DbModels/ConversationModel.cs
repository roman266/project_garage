namespace project_garage.Models.DbModels
{
    public class ConversationModel
    {
        public string Id { get; set; }
        public string User1Id { get; set; }
        public string User2Id { get; set; }
        public DateTime StartedAt { get; set; }

        public UserModel User1 { get; set; }
        public UserModel User2 { get; set; }
        public ICollection<MessageModel> Messages { get; set; }
    }
}
