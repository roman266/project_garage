using project_garage.Models.ViewModels;

namespace project_garage.Interfaces.IService
{
    public interface ITokenService
    {
        string GenerateAccessToken(string userId, string email);
        Task<AuthDto> StartSessionAsync(string userId, string email);
        Task<string> CreateAndStoreRefreshTokenAsync(string userId, string email);
        Task<AuthDto> RenewAccessTokenAsync();
        Task RevokeRefreshTokenAsync(string refreshToken);
    }
}
