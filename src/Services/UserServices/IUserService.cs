using TodoAPI.src.DTOs;
using TodoAPI.src.Entities;
namespace TodoAPI.src.Services.UserServices;

public interface IUserService
{
    Task<UserEntity?> GetByUsernameAsync(string username, CancellationToken cancellationToken = default);
    Task<UserEntity?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task CreateAsync(RegisterDTO dto, CancellationToken cancellationToken = default);
    Task UpdateAsync(LoginDTO dto, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
