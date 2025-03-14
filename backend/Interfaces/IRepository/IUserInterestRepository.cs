using project_garage.Models.DbModels;
using project_garage.Models.Enums;

namespace project_garage.Interfaces.IRepository
{
    public interface IUserInterestRepository
    {
        public Task AddInterestAsync(UserInterestModel userInterest);
        public Task<bool> UserHasInterestAsync(string userId, UserInterest interest);
        public Task<UserInterestModel> GetInterestByIdAsync(string interestId);
        public Task<List<UserInterestModel>> GetInterestsByUserIdAsync(string userId);
        public Task RemoveInterestAsync(string interestId);
    }
}
