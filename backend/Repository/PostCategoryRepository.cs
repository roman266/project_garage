using Microsoft.EntityFrameworkCore;
using project_garage.Data;
using project_garage.Interfaces.IRepository;
using project_garage.Models.DbModels;

namespace project_garage.Repository
{
    public class PostCategoryRepository : IPostCategoryRepository
    {
        private readonly ApplicationDbContext _context;

        public PostCategoryRepository(ApplicationDbContext context) 
        {
            _context = context; 
        }

        public async Task<List<PostCategoryModel>> GetAllAsync()
        {
            var categories = await _context.PostCategories.ToListAsync();
            return categories;
        }
    }
}
