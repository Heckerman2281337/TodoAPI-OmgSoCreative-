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
        public async Task<PagedResult<TaskResponseDTO>> GetAllAsync
            (Guid userId, TaskFilterParams taskFilter
            ,TaskSortParams taskSort, TaskPaginationParams taskPagination
            ,CancellationToken cancellationToken = default)
        {
            var result = await taskRepository.GetAllAsync(userId, taskFilter, taskSort, taskPagination, cancellationToken);
            var taskResponses = result.Data.Select(task => new TaskResponseDTO(task)).ToArray();

            return new PagedResult<TaskResponseDTO>(taskResponses, result.TotalCount);
        }

        public async Task CreateAsync(TaskDTO dto, Guid userId, CancellationToken cancellationToken = default)
        {
            taskValidator.Validate(dto);

            var task = new TaskEntity(dto.Title, dto.Description ?? string.Empty, 
                userId, dto.Deadline, dto.Category, dto.Priority);
            await taskRepository.CreateAsync(task, cancellationToken);
        }

        public async Task<TaskResponseDTO> GetByIdAsync(Guid id, Guid userId, CancellationToken cancellationToken = default)
        {
            var taskEntity = await taskRepository.GetByIdAsync(id, cancellationToken);
            if (taskEntity is null)
                throw new KeyNotFoundException("Задача не найдена.");
            if (taskEntity.UserId != userId)
                throw new KeyNotFoundException("Задача не найдена.");

            var taskResponse = new TaskResponseDTO(taskEntity);

            return taskResponse;
        }

        public async Task DeleteAsync(Guid id, Guid userId, CancellationToken cancellationToken = default)
        {
            var task = await taskRepository.GetByIdAsync(id, cancellationToken);

            if (task is null)
                throw new KeyNotFoundException("Задача не найдена.");
            if (task.UserId != userId)
                throw new KeyNotFoundException("Задача не найдена.");

            await taskRepository.DeleteAsync(task, cancellationToken);
        }

        public async Task<TaskResponseDTO> UpdateAsync(UpdateTaskDTO dto, Guid id, Guid userId, CancellationToken cancellationToken = default)
        {
            updatedTaskValidator.Validate(dto);
            var taskEntity = await taskRepository.GetByIdAsync(id, cancellationToken);
            
            if (taskEntity is null)
                throw new KeyNotFoundException("Задача не найдена.");
            if (taskEntity.UserId != userId)
                throw new KeyNotFoundException("Задача не найдена.");

            taskEntity.Update(dto.Title, dto.Description, dto.IsCompleted,
                dto.Category, dto.Priority, dto.Deadline);

            await taskRepository.UpdateAsync(taskEntity, cancellationToken);

            var taskResponse = new TaskResponseDTO(taskEntity);

            return taskResponse;
        }
    }
}
