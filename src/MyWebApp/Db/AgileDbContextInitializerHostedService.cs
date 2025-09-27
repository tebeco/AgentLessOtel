using Microsoft.EntityFrameworkCore;
using MyWebApp.Db;

namespace MyWebApp.Db;

public class AgileDbContextInitializerHostedService(
    IServiceProvider _serviceProvider,
    ILogger<AgileDbContextInitializerHostedService> _logger)
    : IHostedService
{

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        try
        {
            using var scope = _serviceProvider.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<AgileDbContext>();

            await dbContext.Database.MigrateAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while migrating or initializing the database.");
            throw;
        }
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}