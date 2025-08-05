public class SensorProcessor : BackgroundService
{
    private readonly ILogger<SensorProcessor> _logger;
    private readonly ILogger<DataService> _sensorControllerLogger;

    public SensorProcessor(ILogger<SensorProcessor> logger, ILogger<DataService> sensorControllerLogger)
    {
        _logger = logger;
        _sensorControllerLogger = sensorControllerLogger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            _logger.LogInformation("----------------------------------");
            _logger.LogInformation("Starting sensor data processing...");
            _logger.LogInformation("----------------------------------");

            try
            {
                // Wywołanie metody przetwarzającej dane
                var controller = new DataService(_sensorControllerLogger);
                //await controller.ProcessSensorsData();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred during background sensor processing.");
            }

            await Task.Delay(TimeSpan.FromMinutes(30), stoppingToken); // Run every 30mins - as data pull from DataFetcher
        }
    }
}
