using TodoAPI.Application.QueryParams;
using TodoAPI.Application.DTOs;

namespace TodoAPI.Application.Services
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
