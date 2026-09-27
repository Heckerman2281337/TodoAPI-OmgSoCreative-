using TodoAPI.Domain.Entities;

namespace TodoAPI.Application.Repo
{
    public interface IUserRepo
    {
        Task<UserEntity?> GetByUsernameAsync(string username, CancellationToken cancellationToken = default);
        Task<UserEntity?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<UserEntity> CreateAsync(UserEntity entity, CancellationToken cancellationToken = default);
        Task<UserEntity?> UpdateAsync(UserEntity entity, CancellationToken cancellationToken = default);
        Task DeleteAsync(UserEntity entity, CancellationToken cancellationToken = default);
    }
}
