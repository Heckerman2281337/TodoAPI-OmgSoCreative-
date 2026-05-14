using TodoAPI.src.Entities;

namespace TodoAPI.src.Repo.TaskRepo
{
    public interface ITaskRepo
    {
        Task<List<TaskEntity>> GetAllAsync();
        Task<TaskEntity?> GetByIdAsync(Guid id);
        Task CreateAsync(TaskEntity entity);
        Task UpdateAsync(TaskEntity entity);
        Task DeleteAsync(Guid id);
    }
}
