﻿using Microsoft.EntityFrameworkCore;
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
                throw new Exception("Somthing goes wrong");
            }

            return request;
        }

        public async Task<List<FriendModel>> GetByUserIdAsync(string userId)
        {
            var list = await _context.Friends.Where(x => x.UserId == userId).ToListAsync();
            return list;
        }

        public async Task SendFriendRequestAsync(FriendModel friendModel)
        {
            _context.Friends.Add(friendModel);
            await _context.SaveChangesAsync();
        }

        public async Task AcceptRequestAsync(FriendModel friendModel)
        {
            friendModel.IsAccepted = true;
            _context.Friends.Update(friendModel);
            await _context.SaveChangesAsync();
        }

        public async Task RejectRequestAsync(FriendModel friendModel)
        {
            await RemoveFriendAsync(friendModel);
        }

        public async Task RemoveFriendAsync(FriendModel friendModel)
        {
            _context.Friends.Remove(friendModel);
            await _context.SaveChangesAsync();
        }
    }
}
