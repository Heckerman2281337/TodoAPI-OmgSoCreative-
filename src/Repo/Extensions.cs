using Microsoft.EntityFrameworkCore;
using TodoAPI.src.Repo.TaskRepository;
using TodoAPI.src.Repo.UserRepository;
using TodoAPI.src.Services.Authentication;

namespace TodoAPI.src.Repo
{
    public static class Extensions
    {
        public static IServiceCollection AddDataAcces(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddScoped<ITaskRepo, TaskRepo>();
            serviceCollection.AddScoped<IUserRepo, UserRepo>();

            serviceCollection.AddDbContext<TodoDbContext>(x =>
            {
                x.UseNpgsql(connectionString: "Host=localhost;Database=ToDoDB;Username=postgres;Password=12345");
            });
            return serviceCollection;
        }
    }
}
