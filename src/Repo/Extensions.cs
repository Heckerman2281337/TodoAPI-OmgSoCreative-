using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using TodoAPI.src.Repo.TaskRepository;
using TodoAPI.src.Repo.UserRepository;
using TodoAPI.src.Services.Authentication;

namespace TodoAPI.src.Repo
{
    public static class Extensions
    {
        public static IServiceCollection AddDataAcces(this IServiceCollection serviceCollection, IConfiguration configuration)
        {
            serviceCollection.AddScoped<ITaskRepo, TaskRepo>();
            serviceCollection.AddScoped<IUserRepo, UserRepo>();

            var connectionString = configuration.GetConnectionString("DefaultConnection");

            serviceCollection.AddDbContext<TodoDbContext>(x =>
            {
                x.UseNpgsql(connectionString);
            });
            return serviceCollection;
        }
    }
}
