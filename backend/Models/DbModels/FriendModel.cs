namespace project_garage.Models.DbModels
{
    public class FriendModel
    {
        public string Id { get; set; }
        public string UserId { get; set; }
        public string FriendId { get; set; }
        public bool IsAccepted { get; set; }
        public UserModel User { get; set; }
        public UserModel Friend { get; set; }
    }
}
