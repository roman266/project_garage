using Microsoft.AspNetCore.Identity;
using project_garage.Models.DbModels;

namespace project_garage.Interfaces.IRepository
{
    public interface IAuthRepository
    {
        Task SignInAsync(UserModel userModel);
        Task SignOutAsync();
    }
}
