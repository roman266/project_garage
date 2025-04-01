namespace project_garage.Models.DTOs
{
    public class ConversationDisplayDto
    {
        public string ConversationId { get; set; }
        public string ProfilePictureUrl { get; set; }
        public string UserName { get; set; }
        public string ActiveStatus { get; set; }
        public bool IsPrivate { get; set; }
        public DateTime StartedAt { get; set; }
    }
}
