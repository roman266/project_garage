using System.ComponentModel.DataAnnotations;

namespace project_garage.Models.ViewModels
{
    public class CreateCommentViewModel
    {
        [Required]
        public Guid PostId { get; set; }

        [Required]
        [StringLength(250)]
        public string Content { get; set; }
    }
}
