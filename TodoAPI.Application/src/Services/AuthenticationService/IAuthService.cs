using TodoAPI.Application.DTOs;

namespace TodoAPI.Application.Services
{
    public interface IAuthService
    {
        public Task<LoginResponseDTO> LoginAsync(LoginDTO dto, CancellationToken cancellationToken = default);
        public Task LogoutAsync(string token, Guid userId, CancellationToken cancellationToken = default);
        public Task<LoginResponseDTO> RefreshAsync(string rawToken, CancellationToken cancellationToken = default);
    }
}
