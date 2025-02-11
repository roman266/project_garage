using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace project_garage.Models.ViewModels
{
    public class RegisterViewModel
    {

        [Required]
        [EmailAddress]
        [JsonPropertyName("email")]
        public string Email { get; set; }

        [Required]
        [JsonPropertyName("username")]
        public string UserName { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [JsonPropertyName("password")]
        public string Password { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [JsonPropertyName("confirmPassword")]
        [Compare("Password", ErrorMessage = "Password don't matches")]
        public string СonfirmPassword { get; set; }
    }
}
