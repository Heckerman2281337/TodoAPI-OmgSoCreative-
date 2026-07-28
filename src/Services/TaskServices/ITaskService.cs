using TodoAPI.DTOs;
using TodoAPI.Entities;
using TodoAPI.QueryParams;

namespace TodoAPI.Services.TaskServices
{
    public interface ITaskService
    {
        Task<PagedResult<TaskResponseDTO>> GetAllAsync
            (Guid userId, TaskFilterParams taskFilter
            ,TaskSortParams taskSort, TaskPaginationParams taskPagination
            ,CancellationToken cancellationToken = default);

        Task CreateAsync(TaskDTO dto, Guid userId, CancellationToken cancellationToken = default);
        Task<TaskResponseDTO> GetByIdAsync(Guid id, Guid userId, CancellationToken cancellationToken = default);
        Task DeleteAsync(Guid id, Guid userId, CancellationToken cancellationToken = default);
        Task<TaskResponseDTO> UpdateAsync(UpdateTaskDTO dto, Guid id, Guid userId, CancellationToken cancellationToken = default);
    }
}
