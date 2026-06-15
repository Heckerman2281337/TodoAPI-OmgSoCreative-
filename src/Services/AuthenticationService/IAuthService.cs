using TodoAPI.src.DTOs;
namespace TodoAPI.src.Services.Authentication
{
    public interface IAuthService
    {
        public Task<string> LoginAsync(LoginDTO dto, CancellationToken cancellationToken = default);
    }
}
