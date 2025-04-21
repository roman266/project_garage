using project_garage.Models.Enums;
using project_garage.Models.DbModels;

namespace project_garage.Interfaces.IService
{
    public interface IUserInterestService
    {
        public Task AddInterestAsync(string userId, List<int> interestIds);
        public Task<List<InterestModel>> GetUserInterestAsync(string userId);
        public Task RemoveInterestAsync(int interestId, string userId);
    }
}
