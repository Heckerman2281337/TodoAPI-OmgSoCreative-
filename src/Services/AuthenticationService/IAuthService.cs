using TodoAPI.DTOs;
namespace TodoAPI.Services.AuthenticationService
{
    public interface IAuthService
    {
        public Task<string> LoginAsync(LoginDTO dto, CancellationToken cancellationToken = default);
    }
}
