using Microsoft.EntityFrameworkCore;
using TodoAPI.src.Entities;

namespace TodoAPI.src.Repo.TaskRepo
{
    public class TaskRepo : ITaskRepo
    {
        public TaskRepo(TaskContext context)
        {
            _context = context;
        }

        private readonly TaskContext _context;

        public async Task<List<TaskEntity>> GetAllAsync()
        {
            return await _context.Tasks.ToListAsync();
        }

        public async Task<TaskEntity?> GetByIdAsync(Guid id)
        {
            return await _context.Tasks.FirstOrDefaultAsync(t => t.Id == id);
        }

        public Task CreateAsync(TaskEntity entity)
        {
            throw new NotImplementedException();
        }

        public Task UpdateAsync(TaskEntity entity)
        {
            throw new NotImplementedException();
        }

        public Task DeleteAsync(Guid id)
        {
            throw new NotImplementedException();
        }
    }
}
