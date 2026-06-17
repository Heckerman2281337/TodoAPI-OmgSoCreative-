using TodoAPI.src.DTOs;
using TodoAPI.src.Services.Authentication;
using TodoAPI.src.Services.TaskServices;
using TodoAPI.src.Services.UserServices;
using TodoAPI.src.Validators;
namespace TodoAPI.src.Services
{
    public static class Extensions
    {
        public static IServiceCollection AddBuisnessLogic(this IServiceCollection serviceCollection)
        {
            
            serviceCollection.AddScoped<IValidator<TaskDTO>, TaskValidator>();
            serviceCollection.AddScoped<IValidator<UpdateTaskDTO>, UpdatedTaskValidator>();
            serviceCollection.AddScoped<IValidator<RegisterDTO>, UserValidator>();

            serviceCollection.AddScoped<IUserService, UserService>();
            serviceCollection.AddScoped<ITaskService, TaskService>();
            serviceCollection.AddScoped<IAuthService, AuthService>();
            return serviceCollection;
        }
    }
}
