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

        public UserInterestService(IUserInterestRepository userInterestRepository, IUserService userService)
        {
            _userInterestRepository = userInterestRepository;
            _userService = userService;
        }

        public async Task AddInterestAsync(string userId, List<string> interestNames)
        {
            if (!await _userService.CheckForExistanceByIdAsync(userId))
                throw new InvalidOperationException($"User with this id: {userId} does not exist");

            foreach (var interestName in interestNames) {
                if (Enum.TryParse<UserInterest>(interestName, true, out var interest) && Enum.IsDefined(typeof(UserInterest), interest))
                {
                    var interestModel = new UserInterestModel
                    {
                        Id = Guid.NewGuid().ToString(),
                        UserId = userId,
                        Interest = interest,
                    };

                    await _userInterestRepository.AddInterestAsync(interestModel);
                }
            }
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
