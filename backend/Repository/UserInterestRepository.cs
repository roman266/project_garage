﻿using Microsoft.EntityFrameworkCore;
using project_garage.Data;
using project_garage.Interfaces.IRepository;
using project_garage.Models.DbModels;
using project_garage.Models.Enums;

namespace project_garage.Repository
{
    public class UserInterestRepository : IUserInterestRepository
    {
        public readonly ApplicationDbContext _context;

        public UserInterestRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AddInterestAsync(UserInterestModel userInteres)
        {
            if (await UserHasInterestAsync(userInteres.UserId, userInteres.InterestId))
                throw new ArgumentException($"User has interest: {userInteres.Interest} already");

            _context.UserInterests.Add(userInteres);
            await _context.SaveChangesAsync();
        }

        public async Task AddInterestRangeAsync(List<UserInterestModel> userInteres)
        {
            _context.UserInterests.AddRange(userInteres);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> UserHasInterestAsync(string userId, int interest)
        {
            return await _context.UserInterests
                .FirstOrDefaultAsync(ui => ui.UserId == userId && ui.InterestId == interest) != null;
        }

        public async Task<UserInterestModel> GetInterestByIdAsync(string interesId)
        {
            var interes = await _context.UserInterests.FirstOrDefaultAsync(ui => ui.Id == interesId);

            if (interes == null)
                throw new KeyNotFoundException("Interes with this id does not exist");

            return interes;
        }

        public async Task<List<InterestModel>> GetInterestsByUserIdAsync(string userId)
        {
            var interests = await _context.UserInterests
                .Where(ui => ui.UserId == userId)
                .Select(ui => ui.Interest)
                .ToListAsync();

            return interests;
        }

        public async Task RemoveInterestAsync(int interesId, string userId)
        {
            var interes = _context.UserInterests
                .FirstOrDefault(ui => ui.InterestId == interesId && ui.UserId == userId);
            _context.UserInterests.Remove(interes);
            await _context.SaveChangesAsync();
        }
    }
}
