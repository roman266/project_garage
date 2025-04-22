using project_garage.Data;
using project_garage.Interfaces.IRepository;
using project_garage.Models.DbModels;

namespace project_garage.Repository
{
    public class InterestRepository : IInterestRepository
    {
        private readonly ApplicationDbContext _context;

        public InterestRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public InterestModel GetInterestById(int id)
        {
            var interest = _context.Interests.FirstOrDefault(i => i.Id == id);
            return interest;
        }

        public List<InterestModel> GetInterestRange(List<int> interestIds)
        {
            var interests = _context.Interests
                .Where(i => interestIds.Contains(i.Id))
                .ToList();
            return interests;
        }

        public List<InterestModel> GetAllInterests()
        {
            var interests = _context.Interests.ToList();
            return interests;
        }

        public void AddInterestRange(List<InterestModel> interests)
        {
            _context.Interests.AddRange(interests);
            _context.SaveChanges();
        }

        public void ClearAllInterests()
        {
            var interests = GetAllInterests();
            _context.Interests.RemoveRange(interests);
            _context.SaveChanges();
        }
    }
}