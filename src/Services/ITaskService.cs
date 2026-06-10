using TodoAPI.src.DTOs;
using TodoAPI.src.Entities;
namespace TodoAPI.src.Services
{
    public interface ITaskService
    {
        Task CreateAsync(TaskDTO dto, CancellationToken cancellationToken = default);

        Task<TaskEntity> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

        Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);

        Task UpdateAsync(TaskDTO dto, Guid id, CancellationToken cancellationToken = default);
    }
}
