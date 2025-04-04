using System.ComponentModel.DataAnnotations;

namespace project_garage.Models.ViewModels
{
    public class EditCommentViewModel
    {
        [Required]
        public int CommentId { get; set; }
        
        [Required]
        [StringLength(250)]
        public string Content { get; set; }

        public string PostId { get; set; }
    }
}
