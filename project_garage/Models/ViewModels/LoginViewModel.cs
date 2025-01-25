﻿using System.ComponentModel.DataAnnotations;

namespace project_garage.Models.ViewModels
{
    public class LoginViewModel
    {
        [Required]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }

    }
}
