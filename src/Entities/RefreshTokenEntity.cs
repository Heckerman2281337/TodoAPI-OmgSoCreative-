using System.Security.Cryptography;
using System.Text;

namespace TodoAPI.Entities
{
    public class RefreshTokenEntity
    {
        private RefreshTokenEntity() { }
        public RefreshTokenEntity(Guid userId, string token)
        {
            Id = Guid.NewGuid();
            UserId = userId;
            TokenHash = token;

            CreatedAt = DateTime.UtcNow;
            ExpiresAt = DateTime.UtcNow.AddDays(7); 
        }

        public Guid Id { get; private set; }
        public string TokenHash { get; private set; } = string.Empty;
        public Guid UserId { get; private set; }
        public DateTime ExpiresAt { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public bool IsRevoked { get; private set; }

        public void RevokeToken()
        {
            IsRevoked = true;
        }
    }
}
