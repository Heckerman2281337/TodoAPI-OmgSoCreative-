using TodoAPI.Application.Validators;
using Microsoft.Extensions.DependencyInjection;
using TodoAPI.Application.Services;
using FluentValidation;

namespace TodoAPI.Application
{
    public static class Extensions
    {
        public static IServiceCollection AddApplication(this IServiceCollection serviceCollection)
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
