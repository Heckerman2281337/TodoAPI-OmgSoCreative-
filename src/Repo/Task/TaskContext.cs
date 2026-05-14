using Microsoft.EntityFrameworkCore;
using TodoAPI.src.Entities;

namespace TodoAPI.src.Repo.TaskRepo
{
    public class TaskContext(DbContextOptions<TaskContext> options) : DbContext(options)
    {
        public DbSet<TaskEntity> Tasks { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<TaskEntity>().HasKey(t => t.Id);
            modelBuilder.Entity<TaskEntity>().Property(t => t.Title).HasMaxLength(140);
            base.OnModelCreating(modelBuilder);
        }
    }
}
