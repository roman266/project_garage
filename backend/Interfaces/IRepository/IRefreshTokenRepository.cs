using project_garage.Models.DbModels;

namespace project_garage.Interfaces.IRepository
{
    public interface IRefreshTokenRepository
    {
        Task AddTokenAsync(RefreshTokenModel refreshToken);
        Task<RefreshTokenModel> GetRefreshTokenAsync(string token, string userId);
        Task<RefreshTokenModel> GetRefreshTokenAsync(string token);
        Task<List<RefreshTokenModel>> GetUserSessionsAsync(string userId);
        Task<RefreshTokenModel> GetRefreshTokenByIdAsync(string tokenId);
        Task UpdateTokenAsync(RefreshTokenModel refreshToken);
    }
}
