﻿using project_garage.Models.DbModels;
using Microsoft.AspNetCore.Identity;


namespace project_garage.Interfaces.IService
{
    public interface IUserService
    {
        Task CreateUserAsync(string userName, string email, string password, string baseUrl);
        Task<UserModel> GetByEmailAsync(string email);
        Task<UserModel> GetByIdAsync(string id);
        Task UpdateUserInfoAsync(string id, string userName, string description);
        Task ConfirmUserEmail(string id, string code);
        Task<bool> CheckPasswordAsync(string id, string password);
        Task DeleteUserAsync(string id);
        Task<List<UserModel>> SearchUsersAsync(string query);
    }
}
