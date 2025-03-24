using System.ComponentModel.DataAnnotations;

namespace project_garage.Models.DTOs
{
    public class ChangePasswordDto
    {
        [Required]
        public string ConfirmationCode { get; set; }

        [Required]
        public string NewPassword { get; set; }
    }
}
