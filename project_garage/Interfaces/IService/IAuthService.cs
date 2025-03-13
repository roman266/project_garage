using project_garage.Models.DbModels;
using project_garage.Models.ViewModels;

namespace project_garage.Interfaces.IService
{
    public interface IAuthService
    {
        Task<AuthDto> SignInAsync(string email, string password);
        Task SignOutAsync();
        
    }
}
