using TodoAPI.Domain.Entities;
using TodoAPI.Application.QueryParams;

namespace TodoAPI.Application.Repo
{
    public interface ITaskRepo
    { 
        Task<PagedResult<TaskEntity>> GetAllAsync
            (Guid userId, TaskFilterParams taskFilter
            ,TaskSortParams taskSort, TaskPaginationParams taskPagination
            ,CancellationToken cancellationToken = default);
        
        Task<TaskEntity?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<TaskEntity> CreateAsync(TaskEntity entity, CancellationToken cancellationToken = default);
        Task<TaskEntity?> UpdateAsync(TaskEntity entity, CancellationToken cancellationToken = default);
        Task DeleteAsync(TaskEntity entity, CancellationToken cancellationToken = default);
    }
}
