using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using TodoAPI.Application.Repo;

namespace TodoAPI.Application.Services
{
    public class TokenCleanUpService(IServiceScopeFactory scopeFactory) : BackgroundService
    {
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                using var scope = scopeFactory.CreateScope();
                var tokenRepo = scope.ServiceProvider.GetRequiredService<ITokenRepo>();
                await tokenRepo.DeleteExpiredAndRevokedAsync(stoppingToken);
                await Task.Delay(TimeSpan.FromHours(12), stoppingToken);
            }
        }
    }
}
