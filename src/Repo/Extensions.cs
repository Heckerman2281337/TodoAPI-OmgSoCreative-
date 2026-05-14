using Microsoft.EntityFrameworkCore;

namespace TodoAPI.src.Repo
{
    public static class Extensions
    {
        public static IServiceCollection AddDataAcces(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddDbContext<AppContext>(x =>
            {
                x.UseNpgsql(connectionString: "Host=localhost;Database=TaskDB;Username=postgres;Password=1234");
            });
            return serviceCollection;
        }
    }
}
