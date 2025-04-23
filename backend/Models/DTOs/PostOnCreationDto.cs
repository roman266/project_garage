using System.ComponentModel.DataAnnotations;

namespace project_garage.Models.ViewModels
{
    public class PostOnCreationDto
    {
        [Required]
        [StringLength(250)]
        public string Description { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Required]
        public IFormFile Image { get; set; }

        [Required]
        public int InterestId { get; set; }
    }
}
