using Microsoft.EntityFrameworkCore;
using TodoAPI.src.Entities;

namespace TodoAPI.src.Repo
{
    public class TodoDbContext(DbContextOptions<TodoDbContext> options) : DbContext(options)
    {
        public DbSet<TaskEntity> Tasks { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(TodoDbContext).Assembly);
            base.OnModelCreating(modelBuilder);
        }
    }
}
