﻿using project_garage.Models.DbModels;

namespace project_garage.Interfaces.IService
{
    public interface IAuthService
    {
        Task<string> SignInAsync(string email, string password);
        Task SignOutAsync();
        
    }
}
