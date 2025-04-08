namespace project_garage.Models.DbModels
{
    public class MessageModel
    {
        public string Id { get; set; }
        public string ConversationId { get; set; }
        public string SenderId { get; set; }
        public string Text { get; set; }
        public string ImageUrl { get; set; }
        public DateTime SendedAt { get; set; }
        public bool IsEdited { get; set; }
        public bool IsReaden {  get; set; }
        public bool IsVisible { get; set; }

        public ConversationModel Conversation { get; set; }
        public UserModel Sender { get; set; }

    }
}
