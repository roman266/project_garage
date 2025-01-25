using System.ComponentModel.DataAnnotations;

namespace project_garage.Models.ViewModels
{
    public class EditViewModel
    {
        [Required]
        public string UserName { get; set; }

        [Required]
        public string Description { get; set; }


    }
}
