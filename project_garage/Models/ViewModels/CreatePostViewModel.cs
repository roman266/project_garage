using System.ComponentModel.DataAnnotations;

namespace project_garage.Models.ViewModels
{
    public class CreatePostViewModel
    {
        [Required]
        [StringLength(50)]
        public string Title { get; set; }
        [Required]
        [StringLength(250)]
        public string Description { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public List<string> ImageUrls { get; set; } = new List<string>();
    }
}
