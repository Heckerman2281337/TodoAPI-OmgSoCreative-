using Microsoft.EntityFrameworkCore;
using TodoAPI.src.Entities;

namespace TodoAPI.src.Repo
{
    public class AppContext(DbContextOptions<AppContext> options) : DbContext(options)
    {
        public DbSet<TaskEntity> Tasks { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppContext).Assembly);
            base.OnModelCreating(modelBuilder);
        }
    }
}
