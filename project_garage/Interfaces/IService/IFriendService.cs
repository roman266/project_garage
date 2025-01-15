namespace project_garage.Interfaces.IService
{
    public interface IFriendService
    {
        Task<int> GetFriendsCount(string id);
    }
}
