using TodoAPI.src.Entities;

namespace TodoAPI.src.Repo.TaskRepository
{
    public interface ITaskRepo
    {
        Task<List<TaskEntity>> GetAllAsync();
        Task<TaskEntity?> GetByIdAsync(Guid id);
        Task<TaskEntity> CreateAsync(TaskEntity entity, CancellationToken cancellationToken);
        Task<TaskEntity> UpdateAsync(TaskEntity entity);
        Task DeleteAsync(Guid id);
    }
}
