using Microsoft.EntityFrameworkCore;
using project_garage.Data;
using project_garage.Interfaces.IRepository;
using project_garage.Models.DbModels;

namespace project_garage.Repository
{
    public class FriendRepository : IFriendRepository
    {
        ApplicationDbContext _context;
        public FriendRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<FriendModel> GetById(string id)
        {
            var request = await _context.Friends.FirstOrDefaultAsync(friend => friend.Id == id);
            if (request == null)
            {
                throw new InvalidDataException("Invalid data");
            }

            return request;
        }

        public async Task<List<FriendModel>> GetByUserIdAsync(string userId)
        {
            var list = await _context.Friends.Where(x => x.UserId == userId || x.FriendId == userId).ToListAsync();
            return list;
        }

        public async Task CreateNewRequestAsync(FriendModel friendModel)
        {
            _context.Friends.Add(friendModel);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateRequestAsync(FriendModel friend)
        {
            _context.Friends.Update(friend);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteFriendAsync(FriendModel friendModel)
        {
            _context.Friends.Remove(friendModel);
            await _context.SaveChangesAsync();
        }
    }
}
