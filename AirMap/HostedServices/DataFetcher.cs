using AirMap.Data;
using Newtonsoft.Json;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using static System.Formats.Asn1.AsnWriter;

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
                Console.WriteLine("\n---------------------------");
                Console.WriteLine($"LookO2 API string: {Look02ApiString + LookO2ApiKey}");
                Console.WriteLine("---------------------------");
                // Pass the required parameters to FetchAndSaveData
                await FetchAndSaveData(Look02ApiString, LookO2ApiKey);
            }
            else
            {
                Console.WriteLine("\n---------------------------");
                Console.WriteLine("LookO2 API string / key is not available.");
                Console.WriteLine("---------------------------");
            }

            // Fetch data from Sensor.Community and save to DB
            string? SensorCommunityApiString = _configuration["SensorCommunityApiString"];
            if (!string.IsNullOrEmpty(SensorCommunityApiString))
            {
                Console.WriteLine("\n---------------------------");
                Console.WriteLine($"SensorCommunity API string: {SensorCommunityApiString}");
                Console.WriteLine("---------------------------");
                // Pass the required parameters to FetchAndSaveData
                await FetchAndSaveData(SensorCommunityApiString, string.Empty);
            }
            else
            {
                Console.WriteLine("\n---------------------------");
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
        var requestUrl = url + apiKey;
        using var request = new HttpRequestMessage(HttpMethod.Get, requestUrl);
        if (!string.IsNullOrEmpty(apiKey))
        {
            request.Headers.Add("X-Api-Key", apiKey);
        }

        var response = await _httpClient.SendAsync(request);
        if (response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync();

            if (url.Contains("looko2"))
            {
                var Source1Data = JsonConvert.DeserializeObject<List<Source1Model>>(content);
                using (var scope = _serviceScopeFactory.CreateScope())
                {
                    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

                    // Filter out duplicates and null/empty Device values
                    var filteredModels = Source1Data?
                        .Where(m => !string.IsNullOrEmpty(m.Device)) // Exclude null/empty Device
                        .GroupBy(m => m.Device) // Group by Device
                        .Select(g => g.First()); // Take the first unique entry
                    // Check for existing devices in the database
                    var existingDevices = dbContext.Source1Models
                        .Select(m => m.Device);

                    // Exclude devices that already exist in the database
                    var newModels = filteredModels
                        .Where(m => !existingDevices.Contains(m.Device));

                    if (Source1Data != null)
                    {
                        var i=1;
                        foreach (var sensor in newModels)
                        {
                            var timeValue = sensor.Epoch == "0" ? null : DateTimeOffset.FromUnixTimeSeconds(long.Parse(sensor.Epoch!)).ToString();

                            if (sensor.Lat.HasValue && sensor.Lon.HasValue)
                            {
                                var Sensor1Models = new Source1Model
                                {
                                    Timestamp = timeValue,
                                    Device = sensor.Device,
                                    PM1 = !string.IsNullOrEmpty(sensor.PM1) ? sensor.PM1 : null,
                                    PM25 = !string.IsNullOrEmpty(sensor.PM25) ? sensor.PM25 : null,
                                    PM10 = !string.IsNullOrEmpty(sensor.PM10) ? sensor.PM10 : null,
                                    Epoch = sensor.Epoch?.ToString(), //-- redundant Epoch value as we convert it to DateTime
                                    Lat = (decimal)sensor.Lat.Value,
                                    Lon = (decimal)sensor.Lon.Value,
                                    Name = sensor.Name,
                                    Indoor = sensor.Indoor?.ToString(),
                                    Temperature = sensor.Temperature?.ToString(),
                                    Humidity = sensor.Humidity?.ToString(),
                                    HCHO = sensor.HCHO?.ToString(),
                                    AveragePM1 = sensor.AveragePM1?.ToString(),
                                    AveragePM25 = sensor.AveragePM25?.ToString(),
                                    AveragePM10 = sensor.AveragePM10?.ToString(),
                                    IJPString = sensor.IJPString,
                                    IJPDescription = sensor.IJPDescription,
                                    Color = sensor.Color,
                                };

                                dbContext.Source1Models.Append(Sensor1Models);
                                await dbContext.SaveChangesAsync();
                                
                                Console.Write(" "+i+" "); //Added as replacement for EF logging - will track NO. of records added
                                i++;
                            }
                            else
                            {
                                Console.WriteLine($"\nSensor data is missing latitude or longitude. Skipped data.\n");
                            }
                        }
                     
                    }
                    else
                        Console.WriteLine("\nSource1Data is empty.\n");
                }
            }
                    
        }
        else
        {
            Console.WriteLine($"\nFailed to fetch data from {url}\n");
            Console.WriteLine($"\nStatus code: {response.StatusCode}\n");
            Console.WriteLine($"\nReason: {response.ReasonPhrase} \n");
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
