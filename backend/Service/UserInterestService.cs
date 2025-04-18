using project_garage.Interfaces.IRepository;
using project_garage.Interfaces.IService;
using project_garage.Models.Enums;
using project_garage.Models.DbModels;
using project_garage.Repository;
using System.Xml.Linq;

namespace project_garage.Service
{
    public class UserInterestService : IUserInterestService
    {
        private readonly IUserInterestRepository _userInterestRepository;
        private readonly IUserService _userService;
        private readonly IInterestRepository _interestRepository;

        public UserInterestService(
            IUserInterestRepository userInterestRepository, 
            IUserService userService, 
            IInterestRepository interestRepository)
        {
            _userInterestRepository = userInterestRepository;
            _userService = userService;
            _interestRepository = interestRepository;
        }

        public async Task AddInterestAsync(string userId, List<int> interestIds)
        {
            if (!await _userService.CheckForExistanceByIdAsync(userId))
                throw new InvalidOperationException($"User with this id: {userId} does not exist");

            var interests = _interestRepository.GetInterestRange(interestIds);

            if (!interests.Any())
                throw new ArgumentException("Invalid interest ids");

            var userInterests = interests.Select(ui => new UserInterestModel
            {
                Id = Guid.NewGuid().ToString(),
                UserId = userId,
                InterestId = ui.Id,
            }).ToList();

            await _userInterestRepository.AddInterestRangeAsync(userInterests);
        }

        public async Task<List<UserInterestModel>> GetUserInterestAsync(string userId)
        {
            if (!await _userService.CheckForExistanceByIdAsync(userId))
                throw new InvalidOperationException($"User with this id: {userId} does not exist");

            var interests = await _userInterestRepository.GetInterestsByUserIdAsync(userId);
            return interests;
        }

        public async Task RemoveInterestAsync(string interestId)
        {
            if (string.IsNullOrEmpty(interestId))
                throw new ArgumentException("Wrong interestId format");

            await _userInterestRepository.RemoveInterestAsync(interestId);
        }
    }
}
