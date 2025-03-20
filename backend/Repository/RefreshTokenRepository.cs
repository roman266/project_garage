using Microsoft.EntityFrameworkCore;
using project_garage.Data;
using project_garage.Interfaces.IRepository;
using project_garage.Models.DbModels;

namespace project_garage.Repository
{
    public class RefreshTokenRepository : IRefreshTokenRepository
    {
        private readonly ApplicationDbContext _context;

        public RefreshTokenRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AddTokenAsync(RefreshTokenModel refreshToken)
        {
            await _context.RefreshTokens.AddAsync(refreshToken);
            await _context.SaveChangesAsync();
        }

        public async Task<List<RefreshTokenModel>> GetUserSessionsAsync(string userId)
        {
            var sessions = await _context.RefreshTokens.Where(rt => rt.UserId == userId).ToListAsync();
            return sessions;
        }

        public async Task<RefreshTokenModel> GetRefreshTokenAsync(string token, string userId)
        {
            var refreshToken = await _context.RefreshTokens.FirstOrDefaultAsync(rt => rt.Token == token &&  rt.UserId == userId);
            if (refreshToken == null)
                throw new ArgumentException($"No token: {token} for user with id {userId} founded");
            return refreshToken;
        }

        public async Task<RefreshTokenModel> GetRefreshTokenAsync(string token)
        {
            var refreshToken = await _context.RefreshTokens.FirstOrDefaultAsync(rt => rt.Token == token);
            if (refreshToken == null)
                throw new ArgumentException($"No token: {token} founded");
            return refreshToken;
        }

        public async Task UpdateTokenAsync(RefreshTokenModel refreshToken)
        {
            _context.RefreshTokens.Update(refreshToken);
            await _context.SaveChangesAsync();
        }

        public async Task<RefreshTokenModel> GetRefreshTokenByIdAsync(string tokenId)
        {
            var token = await _context.RefreshTokens.FirstOrDefaultAsync(rt => rt.Id == tokenId);
            if (token == null)
                throw new ArgumentException($"Token with id: {tokenId} does not exist");
            return token;
        }
    }
}
