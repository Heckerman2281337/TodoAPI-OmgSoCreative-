using Microsoft.EntityFrameworkCore;
using TodoAPI.src.Entities;

namespace TodoAPI.src.Repo.TaskRepo
{
    public class TaskRepo : ITaskRepo
    {
        public TaskRepo(TodoDbContext context)
        {
            _context = context;
        }

        private readonly TodoDbContext _context;

        public async Task<List<TaskEntity>> GetAllAsync()
        {
            return await _context.Tasks.ToListAsync();
        }

        public async Task<TaskEntity?> GetByIdAsync(Guid id)
        {
            return await _context.Tasks.FirstOrDefaultAsync(t => t.Id == id);
        }

        public async Task<TaskEntity> CreateAsync(TaskEntity entity)
        {
            await _context.Tasks.AddAsync(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task<TaskEntity> UpdateAsync(TaskEntity entity)
        {
            _context.Tasks.Update(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task DeleteAsync(Guid id)
        {
            var entity = await _context.Tasks.FirstOrDefaultAsync(x => x.Id == id);
            if (entity == null)
                throw new Exception("Такой задачи не существует");

            _context.Tasks.Remove(entity);
            await _context.SaveChangesAsync();
        }
    }
}
