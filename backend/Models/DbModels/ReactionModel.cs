using project_garage.Models.Enums;

namespace project_garage.Models.DbModels
{
    public class ReactionModel
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string UserId { get; set; } = null!;
        public string ReactionTypeId { get; set; } = null!; 
        public string EntityId { get; set; } = null!;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public UserModel User { get; set; } = null!;
        public ReactionTypeModel ReactionType { get; set; } = null!;
    }
}