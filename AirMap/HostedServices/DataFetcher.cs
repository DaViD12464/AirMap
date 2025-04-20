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
        // Run timer every 30 minutes
        _timer = new Timer(async state => await DoWorkAsync(state), null, TimeSpan.Zero, TimeSpan.FromMinutes(30));
        return Task.CompletedTask;
    }

    private async Task DoWorkAsync(object? state)
    {
        await _semaphore.WaitAsync();
        try
        {
            string? Look02ApiString = _configuration["Look02ApiString"];
            string? LookO2ApiKey = _configuration["LookO2ApiKey"];
            if (!string.IsNullOrEmpty(Look02ApiString) && !string.IsNullOrEmpty(LookO2ApiKey))
            {
                Console.WriteLine("---------------------------");
                Console.WriteLine($"LookO2 API string: {Look02ApiString + LookO2ApiKey}");
                Console.WriteLine("---------------------------");
                // Pass the required parameters to FetchAndSaveData
                await FetchAndSaveData(Look02ApiString, LookO2ApiKey);
            }
            else
            {
                Console.WriteLine("---------------------------");
                Console.WriteLine("LookO2 API string / key is not available.");
                Console.WriteLine("---------------------------");
            }

            // Fetch data from Sensor.Community and save to DB
            string? SensorCommunityApiString = _configuration["SensorCommunityApiString"];
            if (!string.IsNullOrEmpty(SensorCommunityApiString))
            {
                Console.WriteLine("---------------------------");
                Console.WriteLine($"SensorCommunity API string: {SensorCommunityApiString}");
                Console.WriteLine("---------------------------");
                // Pass the required parameters to FetchAndSaveData
                await FetchAndSaveData(SensorCommunityApiString, string.Empty);
            }
            else
            {
                Console.WriteLine("---------------------------");
                Console.WriteLine("SensorCommunity API string / key is not available.");
                Console.WriteLine("---------------------------");
            }
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

    private async Task FetchAndSaveData(string url, string? apiKey)
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
                var source2Models = JsonConvert.DeserializeObject<List<Source2Model>>(content);  // using Source2Model from AppDbContext  instead of Models/SensorSet1
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
                var source1Models = JsonConvert.DeserializeObject<List<Source1Model>>(content); // using Source1Model from AppDbContext instead of Models/SensorSet2
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
