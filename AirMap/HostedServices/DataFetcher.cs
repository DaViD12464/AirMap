using AirMap.Data;
using Newtonsoft.Json;
using Microsoft.Extensions.DependencyInjection;

public class AirQualityHostedService : IHostedService, IDisposable
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private Timer? _timer;
    private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);

    public AirQualityHostedService(HttpClient httpClient, IConfiguration configuration, IServiceScopeFactory serviceScopeFactory)
    {
        _httpClient = httpClient;
        _configuration = configuration;
        _serviceScopeFactory = serviceScopeFactory;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        // Read API key from configuration
        string? apiKey = _configuration["AirMonitorApiKey"];
        Console.WriteLine($"Loaded API Key: {apiKey}");

        // Run timer every 30 minutes
        _timer = new Timer(DoWork, null!, TimeSpan.Zero, TimeSpan.FromMinutes(30));
        return Task.CompletedTask;
    }

    private async void DoWork(object? state)
    {
        await _semaphore.WaitAsync();
        try
        {
            string? Look02ApiKey = _configuration["Look02ApiKey"];
            if (!string.IsNullOrEmpty(Look02ApiKey))
            {
                await FetchAndSaveData("http://api.looko2.com/?method=GetAll&token=XXX", Look02ApiKey);
            }
            else
            {
                Console.WriteLine("---------------------------");
                Console.WriteLine("API key is not available.");
                Console.WriteLine("---------------------------");
            }

            // Fetch data from Sensor.Community and save to DB
            await FetchAndSaveData("https://data.sensor.community/static/v1/data.json", string.Empty);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Exception in DoWork: {ex.Message}");
        }
        finally
        {
            _semaphore.Release();
        }
    }

    private async Task FetchAndSaveData(string url, string apiKey)
    {
        using var request = new HttpRequestMessage(HttpMethod.Get, url);
        if (!string.IsNullOrEmpty(apiKey))
        {
            request.Headers.Add("X-Api-Key", apiKey);
        }

        var response = await _httpClient.SendAsync(request);
        if (response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync();
            //Console.WriteLine($"Response content: {content}");

            if (url.Contains("looko2"))
            {
                // Parse and save LookO2 data
                var source2Models = JsonConvert.DeserializeObject<List<Source2Model>>(content);
                if (source2Models != null)
                {
                    using (var scope = _serviceScopeFactory.CreateScope())
                    {
                        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                        dbContext.Source2Models.AddRange(source2Models);
                        await dbContext.SaveChangesAsync();
                    }
                }
            }
            else if (url.Contains("sensor.community"))
            {
                // Parse and save Sensor.Community data
                var source1Models = JsonConvert.DeserializeObject<List<Source1Model>>(content);
                if (source1Models != null)
                {
                    using (var scope = _serviceScopeFactory.CreateScope())
                    {
                        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                        dbContext.Source1Models.AddRange(source1Models);
                        await dbContext.SaveChangesAsync();
                    }
                }
            }
        }
        else
        {
            Console.WriteLine("---------------------------");
            Console.WriteLine($"Failed to fetch data from {url}");
            Console.WriteLine($"Status code: {response.StatusCode}");
            Console.WriteLine($"Reason: {response.ReasonPhrase}");
            Console.WriteLine("---------------------------");
        }
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _timer?.Change(Timeout.Infinite, 0);
        return Task.CompletedTask;
    }

    public void Dispose()
    {
        _timer?.Dispose();
        _semaphore.Dispose();
    }
}
