
using System.Diagnostics.Contracts;

namespace project_garage.Models.DbModels
{
    public class RefreshTokenModel
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Token { get; set; }
        public string UserId { get; set; }
        public UserModel User { get; set; }
        public DateTime ExpiryDate { get; set; }
        public bool IsRevoked { get; set; }
    }
}