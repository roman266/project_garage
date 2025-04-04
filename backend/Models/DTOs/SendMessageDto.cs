namespace project_garage.Models.DTOs
{
    public class SendMessageDto
    {
        public string ConversationId { get; set; }
        public string Text { get; set; }
        public IFormFile Image { get; set; }
    }
}
