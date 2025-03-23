using System.ComponentModel.DataAnnotations;

namespace project_garage.Models.DTOs
{
    public class ChangeEmailDto
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
    }
}
