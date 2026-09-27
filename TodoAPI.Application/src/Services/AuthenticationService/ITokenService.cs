using TodoAPI.Domain.Entities;
using TodoAPI.Application.DTOs;

namespace TodoAPI.Application.Services
{
    public interface ITokenService
    {
        public Task<string> GenerateRefreshTokenAsync(UserEntity entity, CancellationToken cancellationToken = default);
        public string GenerateAccessJWT(UserEntity entity);
        public Task RevokeAsync(RefreshTokenEntity entity, CancellationToken cancellationToken = default);
        public Task<LoginResponseDTO> RefreshAsync(string rawToken, CancellationToken cancellationToken = default);
        public Task<RefreshTokenEntity> GetTokenAsync(string rawToken, CancellationToken cancellationToken = default);
        public Task DeleteAsync(string rawToken, CancellationToken cancellationToken = default);
    }
}
