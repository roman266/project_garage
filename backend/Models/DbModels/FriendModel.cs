namespace project_garage.Models.DbModels
{
    public class FriendModel
    {
        public string Id { get; set; }
        public string SenderId { get; set; }
        public string ReceiverId { get; set; }
        public bool IsAccepted { get; set; }
        public DateTime DateTime { get; set; } = DateTime.UtcNow;
        public UserModel User { get; set; }
        public UserModel Friend { get; set; }
    }
}
