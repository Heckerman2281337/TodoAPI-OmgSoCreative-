using TodoAPI.Repo.TokenRepository;

namespace TodoAPI.Services.AuthenticationService
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
