using TodoAPI.src.DTOs;
using TodoAPI.src.Entities;
using TodoAPI.src.QuerryParams;
using TodoAPI.src.QueryParams;
namespace TodoAPI.src.Services.TaskServices
{
    public interface ITaskService
    {
        Task<PagedResult<TaskEntity>> GetAllAsync
            (Guid userId, TaskFilterParams taskFilter
            ,TaskSortParams taskSort, TaskPaginationParams taskPagination
            ,CancellationToken cancellationToken = default);

        Task CreateAsync(TaskDTO dto, Guid userId, CancellationToken cancellationToken = default);
        Task<TaskEntity> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
        Task<TaskEntity> UpdateAsync(UpdateTaskDTO dto, Guid id, CancellationToken cancellationToken = default);
    }
}
