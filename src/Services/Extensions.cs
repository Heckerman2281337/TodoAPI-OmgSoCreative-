using FluentValidation;
using TodoAPI.DTOs;
using TodoAPI.Services.AuthenticationService;
using TodoAPI.Services.TaskServices;
using TodoAPI.Services.UserServices;
using TodoAPI.Validators;

namespace TodoAPI.Services
{
    public static class Extensions
    {
        public static IServiceCollection AddBuisnessLogic(this IServiceCollection serviceCollection)
        {
            //Stateless validators
            serviceCollection.AddValidatorsFromAssemblyContaining<TaskValidator>();

            serviceCollection.AddHostedService<TokenCleanUpService>();

            serviceCollection.AddScoped<IUserService, UserService>();
            serviceCollection.AddScoped<ITaskService, TaskService>();
            serviceCollection.AddScoped<IAuthService, AuthService>();
            serviceCollection.AddScoped<ITokenService, TokenService>();
            return serviceCollection;
        }
    }
}
