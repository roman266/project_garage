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

        public async Task<List<FriendModel>> GetByUserIdAsync(string userId)
        {
            return await _context.Friends
                .Where(f => f.UserId == userId && f.FriendId == userId)
                .ToListAsync();
        }

        public async Task<FriendModel> GetByIdAsync(string id)
        {
            var request = await _context.Friends.FirstOrDefaultAsync(friend => friend.Id == id);
            return request;
        }

        public async Task<List<FriendModel>> GetFriendsAsync(string userId, string? lastRequestId, int limit)
        {
            var requests = await _context.Friends
                .Where(f => ( f.UserId == userId || f.FriendId == userId)  && f.IsAccepted == true &&
                    (lastRequestId == null ||
                      f.DateTime < _context.Friends
                      .Where(fr => fr.Id == lastRequestId)
                      .Select(fr => fr.DateTime)
                      .FirstOrDefault()))
                .Take(20)
                .OrderByDescending(f => f.DateTime)
                .ToListAsync();

            return requests;
        }

        public async Task<List<FriendModel>> GetIncomingRequestsAsync(string userId, string? lastRequestId, int limit)
        {
            var requests = await _context.Friends
                .Where(f => f.FriendId == userId && f.IsAccepted == false &&
                    (lastRequestId == null ||
                      f.DateTime < _context.Friends
                      .Where(fr => fr.Id == lastRequestId)
                      .Select(fr => fr.DateTime)
                      .FirstOrDefault()))
                .Take(20)
                .OrderByDescending(f => f.DateTime)
                .ToListAsync();

            return requests;
        } 
        
        public async Task<List<FriendModel>> GetOutcomingRequestsAsync(string userId, string? lastRequestId, int limit)
        {
            var requests = await _context.Friends
                .Where(f => f.UserId == userId && f.IsAccepted == false &&
                    (lastRequestId == null ||
                      f.DateTime < _context.Friends
                      .Where(fr => fr.Id == lastRequestId)
                      .Select(fr => fr.DateTime)
                      .FirstOrDefault()))
                .Take(20)
                .OrderByDescending(f => f.DateTime)
                .ToListAsync();

            return requests;
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
