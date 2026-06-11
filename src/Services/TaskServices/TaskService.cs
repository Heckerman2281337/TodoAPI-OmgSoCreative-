using TodoAPI.src.Repo.TaskRepository;
using TodoAPI.src.Entities;
using TodoAPI.src.DTOs;

namespace TodoAPI.src.Services.TaskServices
{
    public class TaskService(ITaskRepo taskRepository) : ITaskService
    {
        public async Task CreateAsync(TaskDTO dto, CancellationToken cancellationToken = default)
        {
            var task = new TaskEntity(dto.Title, dto.Description);
            await taskRepository.CreateAsync(task, cancellationToken);
        }

        public async Task<TaskEntity> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var task = await taskRepository.GetByIdAsync(id, cancellationToken);

            if (task is null)
                throw new Exception("НЕТЬ ТАКОЙ ЗАДЧКИ ТО");

            return task;
        }

        public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
        {
            await taskRepository.DeleteAsync(id, cancellationToken);
        }

        public async Task UpdateAsync(TaskDTO dto, Guid id, CancellationToken cancellationToken = default)
        {
            var task = await taskRepository.GetByIdAsync(id, cancellationToken);
            
            if (task is null)
                throw new Exception("НЕТЬ ТАКОЙ ЗАДЧКИ ТО");

            task.Update(dto.Title, dto.Description);

            await taskRepository.UpdateAsync(task, cancellationToken); 
        }

    }
}
