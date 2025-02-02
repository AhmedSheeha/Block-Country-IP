using Block_Country_IP.Repository.IRepo;

namespace Block_Country_IP.Utility
{
    public class Cleanup : BackgroundService
    {
        private readonly ICountryRepository _countryRepository;
        private readonly ILogger<Cleanup> _logger;

        public Cleanup(
            ICountryRepository countryRepository,
            ILogger<Cleanup> logger)
        {
            _countryRepository = countryRepository;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("CleanupBackgroundService is starting.");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    _logger.LogInformation("Running cleanup of expired temporary blocks.");
                    int removedCount = await _countryRepository.CleanupExpiredBlocksAsync();
                    _logger.LogInformation("Removed {RemovedCount} expired temporary blocks.", removedCount);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error occurred during cleanup of expired temporary blocks.");
                }

                // Wait for 5 minutes before running again
                await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);
            }

            _logger.LogInformation("CleanupBackgroundService is stopping.");
        }
    }
}
