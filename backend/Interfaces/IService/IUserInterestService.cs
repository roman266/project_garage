using project_garage.Models.Enums;
using project_garage.Models.DbModels;

namespace project_garage.Interfaces.IService
{
    public interface IUserInterestService
    {
        public Task AddInterestAsync(string userId, List<string> interestNames);
        public Task<List<UserInterestModel>> GetUserInterestAsync(string userId);
        public Task RemoveInterestAsync(string interestId);
    }
}
