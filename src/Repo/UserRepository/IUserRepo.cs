using TodoAPI.src.Entities;
using TodoAPI.src.DTOs;

namespace TodoAPI.src.Repo.UserRepository
{
    public interface IUserRepo
    {
        Task<UserEntity?> GetByUsernameAsync(string username, CancellationToken cancellationToken = default);
        Task<UserEntity?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<UserEntity> CreateAsync(UserEntity entity, CancellationToken cancellationToken = default);
        Task<UserEntity?> UpdateAsync(UserEntity entity, CancellationToken cancellationToken = default);
        Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    }
}
