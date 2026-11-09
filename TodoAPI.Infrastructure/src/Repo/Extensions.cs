using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using TodoAPI.Application.Repo;
using Microsoft.EntityFrameworkCore;
using TodoAPI.Infrastructure.Repo;

namespace TodoAPI.Infrastructure
{
    public static class Extensions
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection serviceCollection, IConfiguration configuration)
        {
            serviceCollection.AddScoped<ITaskRepo, TaskRepo>();
            serviceCollection.AddScoped<IUserRepo, UserRepo>();
            serviceCollection.AddScoped<ITokenRepo, TokenRepo>();

            var connectionString = configuration["DB_CONNECTION"];
            serviceCollection.AddDbContext<TodoDbContext>(x =>
            {
                x.UseNpgsql(connectionString);
            });

            var cacheConnectionString = configuration["REDIS_CONNECTION"];
            serviceCollection.AddStackExchangeRedisCache(op =>
            {
                op.Configuration = cacheConnectionString;
                op.InstanceName = "todo:";
            });

            return serviceCollection;
        }

        public static async Task ApplyMigrationsAsync(this IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<TodoDbContext>();
            await db.Database.MigrateAsync();
        }
    }
}
