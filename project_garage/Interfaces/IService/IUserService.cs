using project_garage.Models.DbModels;
using Microsoft.AspNetCore.Identity;


namespace project_garage.Interfaces.IService
{
    public interface IUserService
    {
        Task<IdentityResult> CreateUserAsync(string userName, string email, string password, string baseUrl);
        Task<UserModel> GetByEmailAsync(string email);
        Task<UserModel> GetByIdAsync(string id);
        Task<IdentityResult> UpdateUserInfoAsync(UserModel user);
        Task<IdentityResult> ConfirmUserEmail(UserModel user);
        Task<bool> CheckPasswordAsync(UserModel user, string password);
        Task<IdentityResult> DeleteUserAsync(string id);
        Task<List<UserModel>> SearchUsersAsync(string query);
    }
}
