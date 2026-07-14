using TodoAPI.Repo.TaskRepository;
using TodoAPI.Entities;
using TodoAPI.DTOs;
using TodoAPI.Validators;
using TodoAPI.QueryParams;

namespace TodoAPI.Services.TaskServices
{
    public class TaskService
        (ITaskRepo taskRepository,
        IValidator<TaskDTO> taskValidator,
        IValidator<UpdateTaskDTO> updatedTaskValidator): ITaskService
    {
        public async Task<PagedResult<TaskEntity>> GetAllAsync
            (Guid userId, TaskFilterParams taskFilter
            ,TaskSortParams taskSort, TaskPaginationParams taskPagination
            ,CancellationToken cancellationToken = default)
        {
            return await taskRepository.GetAllAsync(userId, taskFilter, taskSort, taskPagination, cancellationToken);
        }

        public async Task CreateAsync(TaskDTO dto, Guid userId, CancellationToken cancellationToken = default)
        {
            taskValidator.Validate(dto);
            var task = new TaskEntity(dto.Title, dto.Description ?? string.Empty, userId);
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

        public async Task<TaskEntity> UpdateAsync(UpdateTaskDTO dto, Guid id, CancellationToken cancellationToken = default)
        {
            updatedTaskValidator.Validate(dto);
            var task = await taskRepository.GetByIdAsync(id, cancellationToken);
            
            if (task is null)
                throw new ArgumentException("НЕТЬ ТАКОЙ ЗАДАЧКИ ТО");

            task.Update(dto.Title, dto.Description, dto.IsComplete);

            await taskRepository.UpdateAsync(task, cancellationToken); 

            return task;
        }

    }
}
