using Microsoft.EntityFrameworkCore;
using TodoAPI.src.Entities;
using TodoAPI.src.QuerryParams;
using TodoAPI.src.QueryParams;

namespace TodoAPI.src.Repo.TaskRepository
{
    public class TaskRepo : ITaskRepo
    {
        public TaskRepo(TodoDbContext context)
        {
            _context = context;
        }

        private readonly TodoDbContext _context;

        public async Task<PagedResult<TaskEntity>> GetAllAsync
            (Guid userId, TaskFilterParams taskFilter, 
            TaskSortParams taskSort, TaskPaginationParams taskPagination
            ,CancellationToken cancellationToken)
        {
            return await _context.Tasks.Where(x => x.UserId == userId)
                .Filter(taskFilter)
                .Sort(taskSort)
                .ToPagedAsync(taskPagination);
        }

        public async Task<TaskEntity?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
        {
            return await _context.Tasks.FirstOrDefaultAsync(t => t.Id == id);
        }

        public async Task<TaskEntity> CreateAsync(TaskEntity entity, CancellationToken cancellationToken)
        {
            await _context.Tasks.AddAsync(entity, cancellationToken);
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task<TaskEntity?> UpdateAsync(TaskEntity entity, CancellationToken cancellationToken)
        {
            _context.Tasks.Update(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task DeleteAsync(TaskEntity entity, CancellationToken cancellationToken)
        {
            _context.Tasks.Remove(entity);
            await _context.SaveChangesAsync();
        }
    }
}
