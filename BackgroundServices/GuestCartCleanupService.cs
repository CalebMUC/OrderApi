using Minimart_Api.Services.GuestCheckout;

namespace Minimart_Api.BackgroundServices
{
    /// <summary>
    /// Background service to clean up expired guest carts and abandon old pending checkouts
    /// Runs every 6 hours
    /// </summary>
    public class GuestCartCleanupService : BackgroundService
    {
        private readonly ILogger<GuestCartCleanupService> _logger;
        private readonly IServiceProvider _serviceProvider;
        private readonly TimeSpan _interval = TimeSpan.FromHours(6);

        public GuestCartCleanupService(
            ILogger<GuestCartCleanupService> logger,
            IServiceProvider serviceProvider)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Guest Cart Cleanup Service started");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await RunCleanupAsync();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error occurred during guest cart cleanup");
                }

                // Wait for the next interval
                await Task.Delay(_interval, stoppingToken);
            }

            _logger.LogInformation("Guest Cart Cleanup Service stopped");
        }

        private async Task RunCleanupAsync()
        {
            _logger.LogInformation("Starting guest cart cleanup task");

            using var scope = _serviceProvider.CreateScope();
            var guestCartService = scope.ServiceProvider.GetRequiredService<IGuestCartService>();
            var guestCheckoutService = scope.ServiceProvider.GetRequiredService<IGuestCheckoutService>();

            try
            {
                // Clean up expired carts
                var expiredCartsCount = await guestCartService.CleanupExpiredCartsAsync();
                _logger.LogInformation("Cleaned up {Count} expired guest carts", expiredCartsCount);

                // Abandon old pending checkouts
                var abandonedCheckoutsCount = await guestCheckoutService.AbandonOldPendingCheckoutsAsync();
                _logger.LogInformation("Abandoned {Count} old pending checkouts", abandonedCheckoutsCount);

                _logger.LogInformation(
                    "Guest cart cleanup completed. Expired carts: {ExpiredCarts}, Abandoned checkouts: {AbandonedCheckouts}",
                    expiredCartsCount, abandonedCheckoutsCount);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during cleanup task execution");
                throw;
            }
        }

        public override async Task StopAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Guest Cart Cleanup Service is stopping");
            await base.StopAsync(stoppingToken);
        }
    }
}
