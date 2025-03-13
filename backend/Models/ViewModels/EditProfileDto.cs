using System.ComponentModel.DataAnnotations;

namespace project_garage.Models.ViewModels
{
    public class EditProfileDto
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Description { get; set; }
    }
}
