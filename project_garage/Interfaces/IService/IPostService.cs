namespace project_garage.Interfaces.IService
{
    public interface IPostService
    {
        Task<int> GetCountOfPosts(string id);
    }
}
