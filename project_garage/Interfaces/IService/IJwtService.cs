namespace project_garage.Interfaces.IService
{
    public interface IJwtService
    {
        string GenerateToken(string userId, string email);
    }
}
