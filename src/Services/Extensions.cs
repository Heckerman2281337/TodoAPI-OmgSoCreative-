using TodoAPI.src.Services.TaskServices;

namespace TodoAPI.src.Services
{
    public static class Extensions
    {
        public static IServiceCollection AddBuisnessLogic(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddScoped<IUserService, UserService>(); 
            return serviceCollection;
        }
    }
}
