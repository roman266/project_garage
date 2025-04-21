using project_garage.Data;
using project_garage.Models.DbModels;
using project_garage.Interfaces.IRepository;
using project_garage.Repository;

namespace project_garage.Bogus
{
    public static class DbSeeder
    {
        public static void EnsureInterestsConsistent(IServiceProvider services, IConfiguration config)
        {
            using var scope = services.CreateScope();
            var scopedProvider = scope.ServiceProvider;
            var context = scopedProvider.GetRequiredService<ApplicationDbContext>();
            var _interestRepository = scopedProvider.GetRequiredService<IInterestRepository>();
            var predefinedInterests = GetInterestsFromJSON(config);
            var interests = _interestRepository.GetAllInterests();

            if (NeedsRevrite(interests, predefinedInterests))
            {
                _interestRepository.ClearAllInterests();
                _interestRepository.AddInterestRange(predefinedInterests);
            }
        }

        private static bool NeedsRevrite(List<InterestModel> interests, List<InterestModel> predefinedInterests)
        {
            foreach (var predefinedInterest in predefinedInterests)
            {
                var existingInterest = interests.FirstOrDefault(ei => ei.Id == predefinedInterest.Id);

                if (existingInterest == null || existingInterest.Name != predefinedInterest.Name)
                {
                    return true;
                }
            }
            return false;
        }

        private static List<InterestModel> GetInterestsFromJSON(IConfiguration config)
        {
            var predefinedInterests = config
                .GetSection("Interests")
                .Get<List<InterestModel>>();

            if (predefinedInterests == null)
                throw new InvalidCastException("There no \"Interests\" in appsettings.json");

            return predefinedInterests;
        }
    }
}
