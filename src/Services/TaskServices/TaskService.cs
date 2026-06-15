using TodoAPI.src.Repo.TaskRepository;
using TodoAPI.src.Entities;
using TodoAPI.src.DTOs;
using TodoAPI.src.Validators;
namespace TodoAPI.src.Services.TaskServices
{
    public class TaskService(ITaskRepo taskRepository, IValidator<TaskDTO> taskValidator) : ITaskService
    {
        public async Task CreateAsync(TaskDTO dto, Guid userId, CancellationToken cancellationToken = default)
        {
            taskValidator.Validate(dto);
            var task = new TaskEntity(dto.Title, dto.Description, userId);
            await taskRepository.CreateAsync(task, cancellationToken);
        }

        public async Task<TaskEntity> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var task = await taskRepository.GetByIdAsync(id, cancellationToken);

            if (task is null)
                throw new ArgumentException("НЕТЬ ТАКОЙ ЗАДАЧКИ ТО");

            return task;
        }

        public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var entity = await taskRepository.GetByIdAsync(id);

            if (entity == null)
                throw new ArgumentException("Такой задачи не существует");

            await taskRepository.DeleteAsync(entity, cancellationToken);
        }

        public async Task<TaskEntity> UpdateAsync(TaskDTO dto, Guid id, CancellationToken cancellationToken = default)
        {
            taskValidator.Validate(dto);
            var task = await taskRepository.GetByIdAsync(id, cancellationToken);
            
            if (task is null)
                throw new ArgumentException("НЕТЬ ТАКОЙ ЗАДАЧКИ ТО");

            task.Update(dto.Title, dto.Description);

            await taskRepository.UpdateAsync(task, cancellationToken); 

            return task;
        }

    }
}
