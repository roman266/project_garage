using project_garage.Models.DbModels;
using System.Collections.Generic;

namespace project_garage.Models.ViewModels
{
    public class HomeViewModel
    {
        public UserModel User { get; set; }
        public List<PostModel> Posts { get; set; }
        public int FriendsCount { get; set; }
    }
}