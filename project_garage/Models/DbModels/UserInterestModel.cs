using project_garage.Models.Enums;

namespace project_garage.Models.DbModels
{
    public class UserInterestModel
    {
        public string Id { get; set; }
        public string UserId { get; set; }
        public UserModel User { get; set; }
        public UserInterest Interest { get; set; }
    }
}
