using TodoAPI.src.Repo.TaskRepository;
using TodoAPI.src.Entities;
using TodoAPI.src.DTOs;
namespace TodoAPI.src.Services
{
    public class TaskService(ITaskRepo taskRepository) : ITaskService
    {
        public async Task CreateAsync(TaskDTO dto, CancellationToken cancellationToken = default)
        {
            var task = new TaskEntity(dto.Title, dto.Description);
            await taskRepository.CreateAsync(task, cancellationToken);
        }
    }
}
