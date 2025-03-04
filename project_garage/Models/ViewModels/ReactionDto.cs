using project_garage.Models.Enums;

namespace project_garage.Models.ViewModels
{
    public class ReactionDto
    {
        public string UserId { get; set; }
        public EntityType EntityType { get; set; }
        public string EntityId { get; set; }
        public ReactionType ReactionType { get; set; }
    }
}
