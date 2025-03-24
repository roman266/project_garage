using Microsoft.AspNetCore.Identity;
using project_garage.Models.DbModels;

namespace project_garage.Interfaces.IRepository
{
    public interface IUserRepository
    {
        Task<IdentityResult> CreateUserAsync(UserModel userModel, string password);
        Task<UserModel> GetByEmailAsync(string email);
        Task<UserModel> GetByIdAsync(string id);
        Task<IdentityResult> UpdateUserInfoAsync(UserModel user);
        Task<IdentityResult> UpdateUserPasswordAsync(string userId, string password);
        Task<bool> CheckPasswordAsync(UserModel user, string password);
        Task<IdentityResult> DeleteUserAsync(string id);
        Task<List<UserModel>> SearchUsersAsync(string query);
        Task<IdentityResult> UpdateUserEmailAsync(UserModel user, string email);
    }
}
