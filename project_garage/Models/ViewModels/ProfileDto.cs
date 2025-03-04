using project_garage.Models.DbModels;

namespace project_garage.Models.ViewModels
{
    public class ProfileDto
    {
        public string UserId { get; set; }
        public string Nickname { get; set; }
        public string Description { get; set; }
        public int FriendsCount { get; set; }
        public int PostsCount { get; set; }
        public int ReactionsCount { get; set; }
        public bool CanAddFriend { get; set; }
        public List<PostModel> Posts { get; set; }
    }
}
