using Microsoft.AspNetCore.Identity;
using project_garage.Models.DbModels;
using project_garage.Models.ViewModels;

namespace project_garage.Interfaces.IRepository
{
    public interface IUserRepository
    {
        Task<IdentityResult> CreateUserAsync(UserModel userModel, string password);
        Task<UserModel> GetByEmailAsync(string email);
        Task<UserModel> GetByIdAsync(string id);
        Task UpdateUserInfoAsync(UserModel user);
        Task ChangeName(UserModel user, string userName);
        Task<bool> CheckPasswordAsync(UserModel user, string password);
        Task DeleteUserAsync(UserModel user);
        Task<List<UserModel>> SearchByQueryAsync(string query);
        Task<UserModel> FindByNameAsync(string userName);
    }
}
