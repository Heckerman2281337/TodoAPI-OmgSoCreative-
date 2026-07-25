using TodoAPI.DTOs;
using TodoAPI.Entities;

namespace TodoAPI.Services.UserServices
{
    public interface IUserService
    {
        Task<UserEntity?> GetByUsernameAsync(string username, CancellationToken cancellationToken = default);
        Task<UserEntity?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task CreateAsync(RegisterDTO dto, CancellationToken cancellationToken = default);
        Task UpdateAsync(UpdateUserDTO dto, Guid id, CancellationToken cancellationToken = default);
        Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    }
}
