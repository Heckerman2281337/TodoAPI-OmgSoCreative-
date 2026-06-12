using TodoAPI.src.DTOs;
namespace TodoAPI.src.Services.AuthService
{
    public interface IAuthService
    {
        public Task LoginAsync(LoginDTO dto, CancellationToken cancellationToken = default);
    }
}
