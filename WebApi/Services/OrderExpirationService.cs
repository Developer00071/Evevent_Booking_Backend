using Application.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace WebApi.Services;

public class OrderExpirationService(
    IServiceScopeFactory serviceScopeFactory,
    ILogger<OrderExpirationService> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using var timer = new PeriodicTimer(TimeSpan.FromMinutes(1));
        while (await timer.WaitForNextTickAsync(stoppingToken))
        {
            using var scope = serviceScopeFactory.CreateScope();
            var eventBookingServer = scope.ServiceProvider.GetRequiredService<IOrderServer>();
            try
            {
                await eventBookingServer.ExpireOrders();
            }
            catch (Exception exception) when (exception is not OperationCanceledException)
            {
                logger.LogError(exception, "Failed to expire pending orders.");
            }
        }
    }
}
