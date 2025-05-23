﻿using project_garage.Models.DbModels;
using Microsoft.AspNetCore.Identity;
using project_garage.Models.ViewModels;

namespace project_garage.Interfaces.IService
{
    public interface IUserService
    {
        Task<IdentityResult> CreateUserAsync(RegisterDto registerDto);
        Task<IdentityResult> ChangeUserEmailAsync(string password, string email, string userId);
        Task<IdentityResult> ChangeUserPasswordAsync(string userId, string password, string code);
        Task SendPasswordResetEmailAsync(string userId);
        Task<UserModel> GetByEmailAsync(string email);
        Task<UserModel> GetByIdAsync(string id);
        Task<IdentityResult> UpdateUserInfoAsync(string userId, string userName, string firstName, string lastName, string description, string email, string password);
        Task<IdentityResult> UpdateProfilePictureAsync(string userId, string picture);
        Task ConfirmUserEmail(string userId, string code);
        Task<bool> CheckPasswordAsync(UserModel user, string password);
        Task<IdentityResult> DeleteUserAsync(string id);
        Task<List<UserModel>> SearchUsersAsync(string query, string? lastUserId, int limit);
        Task<bool> CheckForExistanceByIdAsync(string userId);
    }
}
