using project_garage.Models.DbModels;

namespace project_garage.Interfaces.IRepository
{
    public interface IInterestRepository
    {
        InterestModel GetInterestById(int id);
        List<InterestModel> GetInterestRange(List<int> interestIds);
        List<InterestModel> GetAllInterests();
        void ClearAllInterests();
        void AddInterestRange(List<InterestModel> interests);
    }
}
