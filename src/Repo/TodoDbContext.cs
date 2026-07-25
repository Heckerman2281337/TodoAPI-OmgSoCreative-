using Microsoft.EntityFrameworkCore;
using TodoAPI.Entities;

namespace TodoAPI.Repo
{
    public class TodoDbContext(DbContextOptions<TodoDbContext> options) : DbContext(options)
    {
        public DbSet<TaskEntity> Tasks { get; set; }
        public DbSet<UserEntity> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(TodoDbContext).Assembly);
            base.OnModelCreating(modelBuilder);
        }
    }
}
