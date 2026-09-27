using TodoAPI.Domain.Entities;

namespace TodoAPI.Application.Repo
{
    public interface ITokenRepo
    {
        public Task CreateAsync(RefreshTokenEntity refreshToken, CancellationToken cancellationToken = default);
        public Task DeleteAsync(RefreshTokenEntity refreshToken, CancellationToken cancellationToken = default);
        public Task<RefreshTokenEntity?> GetByTokenAsync(string refreshToken, CancellationToken cancellationToken = default);
        public Task RevokeTokenAsync(RefreshTokenEntity refreshToken, CancellationToken cancellation = default);
        public Task DeleteExpiredAndRevokedAsync(CancellationToken cancellationToken = default);
    }
}
