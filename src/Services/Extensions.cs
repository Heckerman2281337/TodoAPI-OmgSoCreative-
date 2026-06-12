using TodoAPI.src.Services.TaskServices;
using TodoAPI.src.Services.UserServices;
namespace TodoAPI.src.Services
{
    public static class Extensions
    {
        public static IServiceCollection AddBuisnessLogic(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddScoped<IUserService, UserService>();
            serviceCollection.AddScoped<ITaskService, TaskService>();
            return serviceCollection;
        }
    }
}
