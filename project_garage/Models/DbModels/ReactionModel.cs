using project_garage.Models.Enums;

namespace project_garage.Models.DbModels
{
    public class ReactionModel
    {
        public string Id { get; set; }
        public string UserId { get; set; }
        public ReactionType ReactionType { get; set; }
        public EntityType EntityType { get; set; }
        public string EntityId { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public UserModel User { get; set; } = null!;


    }
}
