﻿using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using project_garage.Interfaces.IRepository;
using project_garage.Models.DbModels;

namespace project_garage.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly UserManager<UserModel> _userManager;
        public UserRepository(UserManager<UserModel> userManager) 
        {
            _userManager = userManager;
        }

        public async Task<IdentityResult> CreateUserAsync(UserModel userModel, string password)
        {
            var user = new UserModel
            {
                UserName = userModel.UserName,
                Email = userModel.Email,
                EmailConfirmationCode = userModel.EmailConfirmationCode,
            };

            var result = await _userManager.CreateAsync(user, password);
            return result;
        }

        public async Task<UserModel> GetByEmailAsync(string email)
        {
            if (string.IsNullOrEmpty(email)) 
            {
                throw new Exception("Wrong email");
            }
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                return null;
            }
            return user;
        }

        public async Task<UserModel> GetByIdAsync(string id)
        {
            var user = await _userManager.FindByIdAsync(id);

            if (user == null)
            {
                throw new Exception("No user found");
            }

            return user;
        }

        public async Task<IdentityResult> UpdateUserInfoAsync(UserModel user)
        {
            var result = await _userManager.UpdateAsync(user);
            return result;
        }
        public async Task UpdateUserStatusAsync(string userId, string status)
        {
            var user = await GetByIdAsync(userId);
            if (user == null)
            {
                throw new Exception("User not found");
            }

            user.ActiveStatus = status;
            user.UpdatedAt = DateTime.UtcNow;
            await _userManager.UpdateAsync(user);
        }

        public async Task<IdentityResult> SetUserNameAsync(UserModel user, string userName)
        {
            return await _userManager.SetUserNameAsync(user, userName);
        }

        public async Task<IdentityResult> UpdateUserPasswordAsync(string userId, string password)
        {
            var user = await GetByIdAsync(userId);
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            return await _userManager.ResetPasswordAsync(user, token, password);
        }

        public async Task<Boolean> CheckPasswordAsync(UserModel user, string password)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }

            var result = await _userManager.CheckPasswordAsync(user, password);
            return result;
        }

        public async Task<IdentityResult> DeleteUserAsync(string id) {
            var user = await GetByIdAsync(id);
            if (user == null)
            {
                throw new InvalidOperationException("User is null here");
            }

            var result = await _userManager.DeleteAsync(user);
            return result;
        }

        public async Task<List<UserModel>> SearchUsersAsync(string query, string? lastUserId, int limit)
        {
            if (string.IsNullOrEmpty(query))
                throw new ArgumentException("Query cannot be null or empty", nameof(query));

            var usersQuery = _userManager.Users
                .Where(u => u.UserName.Contains(query));

            if (!string.IsNullOrEmpty(lastUserId))
            {
                usersQuery = usersQuery
                    .Where(u => string.Compare(u.Id, lastUserId) > 0); // <-- Порівнюємо по Id
            }

            var users = await usersQuery
                .OrderBy(u => u.Id) // <-- Сортуємо по Id
                .Take(limit)
                .Select(u => new UserModel
                {
                    Id = u.Id,
                    UserName = u.UserName,
                    FirstName = u.FirstName,
                    LastName = u.LastName,
                    ProfilePicture = u.ProfilePicture,
                    ActiveStatus = u.ActiveStatus
                })
                .ToListAsync();

            return users;
        }




        public async Task<IdentityResult> UpdateUserEmailAsync(UserModel user, string email)
        {
            var result = await _userManager.SetEmailAsync(user, email);
            return result;
        }
    }
}
