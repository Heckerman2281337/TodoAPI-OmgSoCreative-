using TodoAPI.Repo.TaskRepository;
using TodoAPI.Entities;
using TodoAPI.DTOs;
using TodoAPI.Validators;
using TodoAPI.QueryParams;
using Serilog;
using FluentValidation;

namespace TodoAPI.Services.TaskServices
{
    public class TaskService
        (ITaskRepo taskRepository,
        IValidator<TaskDTO> taskValidator,
        IValidator<UpdateTaskDTO> updatedTaskValidator): ITaskService
    {
        private static readonly Serilog.ILogger _log = Log.ForContext<TaskService>();

        public async Task<PagedResult<TaskResponseDTO>> GetAllAsync
            (Guid userId, TaskFilterParams taskFilter
            ,TaskSortParams taskSort, TaskPaginationParams taskPagination
            ,CancellationToken cancellationToken = default)
        {
            var result = await taskRepository.GetAllAsync(userId, taskFilter, taskSort, taskPagination, cancellationToken);
            var taskResponses = result.Data.Select(task => new TaskResponseDTO(task)).ToArray();
            _log.Information("Выдача всех задач для пользователя: {UserId}", userId);
            return new PagedResult<TaskResponseDTO>(taskResponses, result.TotalCount);
        }

        public async Task CreateAsync(TaskDTO dto, Guid userId, CancellationToken cancellationToken = default)
        {
            taskValidator.Validate(dto);
            var task = new TaskEntity(dto.Title, dto.Description ?? string.Empty, 
                userId, dto.Deadline, dto.Category, dto.Priority);
            
            await taskRepository.CreateAsync(task, cancellationToken);
            _log.Information("Задача {TaskId} была успешно создана", task.Id);
        }

        public async Task<TaskResponseDTO> GetByIdAsync(Guid id, Guid userId, CancellationToken cancellationToken = default)
        {
            var taskEntity = await taskRepository.GetByIdAsync(id, cancellationToken);
            if (taskEntity is null)
            {
                _log.Error("Пользователь {UserId} запросил задачу, которой не существует", userId);
                throw new KeyNotFoundException("Задача не найдена.");
            }
            if (taskEntity.UserId != userId)
            {
                _log.Error("Пользователь {UserId} запросил задачу, которая" +
                    " не принадлежит ему", userId);
                throw new KeyNotFoundException("Задача не найдена.");
            }
            var taskResponse = new TaskResponseDTO(taskEntity);
            _log.Information("Задача {TaskId} возвращается пользователю {UserId}", taskEntity.Id, userId);
            return taskResponse;
        }

        public async Task DeleteAsync(Guid id, Guid userId, CancellationToken cancellationToken = default)
        {
            var task = await taskRepository.GetByIdAsync(id, cancellationToken);

            if (task is null)
            {
                _log.Error("Пользователь {UserId} попытался удалить задачу, которой не существует", userId);
                throw new KeyNotFoundException("Задача не найдена.");
            }
            if (task.UserId != userId)
            {
                _log.Error("Пользователь {UserId} попытался удалить задачу, которая" +
                    " ему не принадлежит {TaskId}", userId, task.Id);
                throw new KeyNotFoundException("Задача не найдена.");
            }

            
            await taskRepository.DeleteAsync(task, cancellationToken);
            _log.Information("Задача {TaskId} успешно удалена у пользователя {UserId}", task.Id, userId);
        }

        public async Task<TaskResponseDTO> UpdateAsync(UpdateTaskDTO dto, Guid id, Guid userId, CancellationToken cancellationToken = default)
        {
            updatedTaskValidator.Validate(dto);
            var taskEntity = await taskRepository.GetByIdAsync(id, cancellationToken);

            if (taskEntity is null)
            {
                _log.Error("Пользователь {UserId} попытался обновить задачу, которой не существует", userId);
                throw new KeyNotFoundException("Задача не найдена.");
            }
            if (taskEntity.UserId != userId)
            {
                _log.Error("Пользователь {UserId} попытался обновить задачу, которая" +
                    " ему не принадлежит", userId);
                throw new KeyNotFoundException("Задача не найдена.");
            }
            taskEntity.Update(dto.Title, dto.Description, dto.IsCompleted,
                dto.Category, dto.Priority, dto.Deadline);
            await taskRepository.UpdateAsync(taskEntity, cancellationToken);
            _log.Information("Пользователь {UserId} обновил задачу {TaskId}", userId, taskEntity.Id);
            var taskResponse = new TaskResponseDTO(taskEntity);

            return taskResponse;
        }
    }
}
