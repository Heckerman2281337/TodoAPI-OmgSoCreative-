using TodoAPI.src.Services.Authentication;
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
            serviceCollection.AddScoped<IAuthService, AuthService>();
            return serviceCollection;
        }
    }
}
