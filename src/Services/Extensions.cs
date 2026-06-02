namespace TodoAPI.src.Services
{
    public static class Extensions
    {
        public static IServiceCollection AddBuisnessLogic(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddScoped<ITaskService, TaskService>(); 
            return serviceCollection;
        }
    }
}
