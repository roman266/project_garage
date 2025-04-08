using project_garage.Models.DbModels;

namespace project_garage.Interfaces.IRepository
{
    public interface IPostCategoryRepository
    {
        Task<List<PostCategoryModel>> GetAllAsync();
    }
}
