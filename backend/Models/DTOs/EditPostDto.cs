using System.ComponentModel.DataAnnotations;

namespace project_garage.Models.ViewModels
{
    public class EditPostDto
    {
        [Required]
        public string PostId { get; set; }
        [Required]
        public string Description { get; set; }
    }
}
