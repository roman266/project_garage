using project_garage.Models.DbModels;

namespace project_garage.Interfaces.IService
{
    public interface IAuthService
    {
        Task<UserModel> SignInAsync(string email, string password);
        Task SignOutAsync();
        
    }
}
