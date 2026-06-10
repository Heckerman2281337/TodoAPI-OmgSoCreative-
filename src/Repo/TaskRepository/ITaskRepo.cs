using TodoAPI.src.Entities;

namespace TodoAPI.src.Repo.TaskRepository
{
    public interface ITaskRepo
    {
        Task<List<TaskEntity>> GetAllAsync();
        Task<TaskEntity?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<TaskEntity> CreateAsync(TaskEntity entity, CancellationToken cancellationToken = default);
        Task<TaskEntity> UpdateAsync(TaskEntity entity, CancellationToken cancellationToken = default);
        Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    }
}
