using project_garage.Models.DbModels;
using project_garage.Models.Enums;

namespace project_garage.Interfaces.IRepository
{
    public interface IUserInterestRepository
    {
        public Task AddInterestAsync(UserInterestModel userInterest);
        public Task AddInterestRangeAsync(List<UserInterestModel> userInterests);
        public Task<bool> UserHasInterestAsync(string userId, int interest);
        public Task<UserInterestModel> GetInterestByIdAsync(string interestId);
        public Task<List<InterestModel>> GetInterestsByUserIdAsync(string userId);
        public Task RemoveInterestAsync(int interestId, string userId);
    }
}
