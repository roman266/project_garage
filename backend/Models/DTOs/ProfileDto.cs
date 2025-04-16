using project_garage.Models.DbModels;

namespace project_garage.Models.ViewModels
{
    public class ProfileDto
    {
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Description { get; set; }
        public string ProfilePicture {  get; set; }
        public int FriendsCount { get; set; }
        public int PostsCount { get; set; }
        public int ReactionsCount { get; set; }
        public bool CanAddFriend { get; set; }
        public List<PostModel> Posts { get; set; }
        public string ActiveStatus { get; set; }
    }
}
