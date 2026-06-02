using TodoAPI.src.DTOs;

namespace TodoAPI.src.Services
{
    public interface ITaskService
    {
        Task CreateAsync(TaskDTO dto, CancellationToken cancellationToken = default);
    }
}
