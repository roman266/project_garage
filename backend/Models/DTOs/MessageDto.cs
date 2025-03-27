namespace project_garage.Models.ViewModels
{
    public class MessageDto
    {
        public string Id { get; set; }
        public string SenderId { get; set; }
        public string SenderName { get; set; }
        public string SenderProfilePicture { get; set; }
        public string Text { get; set; }
        public DateTime SendedAt { get; set; }
        public bool IsReaden { get; set; }
        public bool IsVisible { get; set; }
        public bool IsCurrentUser { get; set; }
    }
}
