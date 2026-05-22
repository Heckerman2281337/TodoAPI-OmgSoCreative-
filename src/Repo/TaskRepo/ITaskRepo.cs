using TodoAPI.src.Entities;

namespace TodoAPI.src.Repo.TaskRepo
{
    public interface ITaskRepo
    {
        Task<List<TaskEntity>> GetAllAsync();
        Task<TaskEntity?> GetByIdAsync(Guid id);
        Task<TaskEntity> CreateAsync(TaskEntity entity);
        Task<TaskEntity> UpdateAsync(TaskEntity entity);
        Task DeleteAsync(Guid id);
    }
}
