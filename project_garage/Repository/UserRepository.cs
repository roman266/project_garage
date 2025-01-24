using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using project_garage.Interfaces.IRepository;
using project_garage.Models.DbModels;
using System.Security.Claims;

namespace project_garage.Repository
{
    public class UserRepository : IUserRepository
    {
        public readonly UserManager<UserModel> _userManager;
        public UserRepository(UserManager<UserModel> userManager) 
        {
            _userManager = userManager;
        }

        public async Task<IdentityResult> CreateUserAsync(UserModel userModel, string password)
        {
            var result = await _userManager.CreateAsync(userModel, password);
            return result;
        }

        public async Task<UserModel> GetByEmailAsync(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            return user;
        }

        public async Task<UserModel> GetByIdAsync(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            return user;
        }

        public async Task UpdateUserInfoAsync(UserModel user)
        {
            var result = await _userManager.UpdateAsync(user);
        }

        public async Task UpdateUserPasswordAsync(UserModel user, string password)
        {
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            await _userManager.ResetPasswordAsync(user, token, password);
        }

        public async Task<bool> CheckPasswordAsync(UserModel user, string password)
        {
            var result = await _userManager.CheckPasswordAsync(user, password);
            return result;
        }

        public async Task DeleteUserAsync(UserModel user) 
        {
            var result = await _userManager.DeleteAsync(user);
        }

        public async Task<UserModel> FindByNameAsync(string userName)
        {
            var user = await _userManager.FindByNameAsync(userName);
            return user;
        }

        public async Task<List<UserModel>> SearchByQueryAsync(string query)
        {
            var users = await _userManager.Users
                .Where(u => u.UserName.Contains(query))
                .Take(10)
                .Select(u => new UserModel
                {
                    Id = u.Id,
                    UserName = u.UserName
                })
                .ToListAsync();

            return users;
        }
    }
}
