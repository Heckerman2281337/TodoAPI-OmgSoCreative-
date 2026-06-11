using TodoAPI.src.Entities;

namespace TodoAPI.src.Repo.UserRepository
{
    public interface IUserRepo
    {
        Task<List<UserEntity>> GetAllAsync();
        Task<UserEntity?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<UserEntity> CreateAsync(UserEntity entity, CancellationToken cancellationToken = default);
        Task<UserEntity> UpdateAsync(UserEntity entity, CancellationToken cancellationToken = default);
        Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    }
}
