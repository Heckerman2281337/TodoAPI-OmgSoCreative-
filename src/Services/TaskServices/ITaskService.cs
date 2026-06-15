using TodoAPI.src.DTOs;
using TodoAPI.src.Entities;
namespace TodoAPI.src.Services.TaskServices
{
    public interface ITaskService
    {
        Task CreateAsync(TaskDTO dto, Guid userId, CancellationToken cancellationToken = default);
        Task<TaskEntity> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
        Task<TaskEntity> UpdateAsync(TaskDTO dto, Guid id, CancellationToken cancellationToken = default);
    }
}
