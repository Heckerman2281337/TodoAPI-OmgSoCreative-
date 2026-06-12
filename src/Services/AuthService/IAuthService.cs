using TodoAPI.src.DTOs;
namespace TodoAPI.src.Services.AuthService
{
    public interface IAuthService
    {
        public Task<string> LoginAsync(LoginDTO dto, CancellationToken cancellationToken = default);
    }
}
