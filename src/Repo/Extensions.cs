using Microsoft.EntityFrameworkCore;
using TodoAPI.Repo.TaskRepository;
using TodoAPI.Repo.TokenRepository;
using TodoAPI.Repo.UserRepository;

namespace TodoAPI.Repo
{
    public static class Extensions
    {
        public static IServiceCollection AddDataAcces(this IServiceCollection serviceCollection, IConfiguration configuration)
        {
            serviceCollection.AddScoped<ITaskRepo, TaskRepo>();
            serviceCollection.AddScoped<IUserRepo, UserRepo>();
            serviceCollection.AddScoped<ITokenRepo, TokenRepo>();

            var connectionString = configuration.GetConnectionString("DefaultConnection");

            serviceCollection.AddDbContext<TodoDbContext>(x =>
            {
                x.UseNpgsql(connectionString);
            });
            return serviceCollection;
        }
    }
}
