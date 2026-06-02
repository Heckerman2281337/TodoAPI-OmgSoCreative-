using Microsoft.EntityFrameworkCore;
using TodoAPI.src.Repo.TaskRepository;

namespace TodoAPI.src.Repo
{
    public static class Extensions
    {
        public static IServiceCollection AddDataAcces(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddScoped<ITaskRepo, TaskRepo>();
            serviceCollection.AddDbContext<TodoDbContext>(x =>
            {
                x.UseNpgsql(connectionString: "Host=localhost;Database=ToDoDB;Username=postgres;Password=1234");
            });
            return serviceCollection;
        }
    }
}
