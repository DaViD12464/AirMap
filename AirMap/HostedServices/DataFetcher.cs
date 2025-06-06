using AirMap.Data;
using AirMap.DTOs;
using AirMap.Helper;
using Newtonsoft.Json;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using static System.Formats.Asn1.AsnWriter;
using AirMap.Models;

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
            string? SensorCommunityApiString = "";//_configuration["SensorCommunityApiString"];
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
                Console.WriteLine("\n\nAdding LookO2 data to DB...\n\n");
                var lookO2Data = JsonConvert.DeserializeObject<List<LookO2Dto>>(content);
                var lookO2DataModel = lookO2Data!.Select(SensorModel.FromDto).ToList();
                using (var scope = _serviceScopeFactory.CreateScope())
                {
                    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

                    // Filter out duplicates and null/empty Device values
                    var filteredModels = lookO2DataModel?
                        .Where(m => !string.IsNullOrEmpty(m.Device)) // Exclude null/empty Device
                        .Where(m => !m.Latitude.Equals(0.0) || !m.Longitude.Equals(0.0)) //ensure devices with Lat == 0 and Lon == 0 are not included
                        .GroupBy(m => m.Device) // Group by Device
                        .SelectMany(g => g);
                    // Check for existing devices in the database
                    var existingDevices = DatabaseHelper.GetAll<SensorModel>(dbContext).Select(m => m.Device);
                    //var existingDevices = dbContext.LookO2Dtos
                    //    .Select(m => m.Device);

                    // Exclude devices that already exist in the database
                    var newModels = filteredModels!
                        .Where(m => !existingDevices.Contains(m.Device));

                    if (lookO2DataModel != null)
                    {
                        var i=1;
                        foreach (var sensor in newModels)
                        {
                            //var timeValue = sensor.Epoch == "0" ? null : DateTimeOffset.FromUnixTimeSeconds(long.Parse(sensor.Epoch!)).ToString();

                            if (!sensor.Latitude.Equals(0.0) || !sensor.Longitude.Equals(0.0))
                            {
                                var sensorModel = new SensorModel
                                {
                                    Device = sensor.Device,
                                    PM1 = sensor.PM1,
                                    PM25 = sensor.PM25,
                                    PM10 = sensor.PM10,
                                    Timestamp = sensor.Timestamp,
                                    Latitude = sensor.Latitude,
                                    Longitude = sensor.Longitude,
                                    Name = sensor.Name,
                                    Indoor = sensor.Indoor,
                                    Temperature = sensor.Temperature,
                                    Humidity = sensor.Humidity,
                                    HCHO = sensor.HCHO,
                                    AveragePM1 = sensor.AveragePM1,
                                    AveragePM25 = sensor.AveragePM25,
                                    AveragePM10 = sensor.AveragePM10,
                                    IJPString = sensor.IJPString,
                                    IJPDescription = sensor.IJPDescription,
                                    Color = sensor.Color,
                                };
                                var existingSensor = await dbContext.SensorModel.FirstOrDefaultAsync(s => s.Device == sensorModel.Device);
                                var addOrUpdate = "";
                                if (existingSensor != null)
                                {
                                    dbContext.SensorModel.Update(existingSensor);
                                    addOrUpdate = "U:";
                                }
                                else
                                {
                                    dbContext.SensorModel.Add(sensorModel);
                                    addOrUpdate = "A:";
                                }
                                try
                                {
                                    await dbContext.SaveChangesAsync();
                                }
                                catch (DbUpdateException ex)
                                {
                                    Console.WriteLine(ex.InnerException?.Message ?? ex.Message);
                                    Environment.Exit(1);
                                }

                                Console.Write(" "+addOrUpdate+i+" "); //Added as replacement for EF logging - will track NO. of records added
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

            //if (url.Contains("sensor.community"))
            //{
            //    Console.WriteLine("\n\nAdding sensor.community data to DB...\n\n");
            //    var Source2Data = JsonConvert.DeserializeObject<List<Source2Model>>(content);
            //    using (var scope = _serviceScopeFactory.CreateScope())
            //    {
            //        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            //        var filteredModels = Source2Data?
            //            .Where(m => m.Location != null && m.SensorDataValues != null)
            //            .Where(m => m.Location!.Latitude != 0 || m.Location!.Longitude != 0)
            //            .GroupBy(m =>
            //                $"{m.Sensor!.Pin}_{m.Location!.Latitude}_{m.Location!.Longitude}") // Unique by Pin + Lat/Lon
            //            .Select(g => g.First());

            //        // Check existing devices in DB (might want a better uniqueness check - currently as combination of pin_lat_lon instead of Id)
            //        var existingDevices = dbContext.Source2Models
            //            .Select(m => new
            //            {
            //                m.Sensor!.Pin,
            //                m.Location!.Latitude,
            //                m.Location.Longitude
            //            });

            //        var newModels = filteredModels!
            //            .Where(m => !existingDevices.Any(e =>
            //                e.Pin == m.Sensor!.Pin &&
            //                e.Latitude == m.Location!.Latitude &&
            //                e.Longitude == m.Location!.Longitude));
            //        if (Source2Data != null)
            //        {
            //            var i = 1;
            //            foreach (var sensor in newModels)
            //            {
            //                if (sensor.Location?.Latitude != null && sensor.Location?.Longitude != null)
            //                {
            //                    var sensor2Model = new Source2Model  //TODO: fix adding pulled Id's data to database instead of identity inserted by EF/SQL DB (Identity Insert error)
            //                    {
            //                        //Id = sensor.Id,
            //                        SamplingRate = sensor.SamplingRate,
            //                        Timestamp = sensor.Timestamp,
            //                        Location = new AirMap.Data.Location
            //                        {
            //                            Id = sensor.Location.Id,
            //                            Latitude = sensor.Location.Latitude,
            //                            Longitude = sensor.Location.Longitude,
            //                            Altitude = sensor.Location.Altitude,
            //                            Country = sensor.Location.Country,
            //                            Indoor = sensor.Location.Indoor,
            //                            ExactLocation = sensor.Location.ExactLocation
            //                        },
            //                        Sensor = new AirMap.Data.Sensor
            //                        {
            //                            Id = sensor.Sensor!.Id.GetUniqueId(),
            //                            Pin = sensor.Sensor!.Pin,
            //                            SensorType = sensor.Sensor.SensorType == null ? null : new AirMap.Data.SensorType()
            //                            {
            //                                //Id= sensor.Sensor.SensorType.Id,
            //                                Name = sensor.Sensor.SensorType.Name,
            //                                Manufacturer = sensor.Sensor.SensorType.Manufacturer
            //                            }
            //                        },
            //                        SensorDataValues = sensor.SensorDataValues!.Select(dataValue => new SensorDataValue
            //                        {
            //                            Id = dataValue.Id.GetUniqueId(),
            //                            Value = dataValue.Value,
            //                            ValueType = dataValue.ValueType
            //                        }).ToList()

            //                    };
            //                    var existingSensor = await dbContext.Source2Models.FirstOrDefaultAsync(s => (s.Location!.Latitude == sensor2Model.Location!.Latitude) && (s.Location!.Longitude == sensor2Model.Location!.Longitude));
            //                    var addOrUpdate = "";
            //                    if (existingSensor != null)
            //                    {
            //                        dbContext.Source2Models.Update(existingSensor);
            //                        addOrUpdate = "U:";
            //                    }
            //                    else
            //                    {
            //                        dbContext.Source2Models.Add(sensor2Model);
            //                        addOrUpdate = "A:";
            //                    }
            //                    try
            //                    {
            //                        await dbContext.SaveChangesAsync();
            //                    }
            //                    catch (DbUpdateException ex)
            //                    {
            //                        Console.WriteLine(ex.InnerException?.Message ?? ex.Message);
            //                        Environment.Exit(1);
            //                    }
            //                    Console.Write(" " + addOrUpdate + i + " "); //Added as replacement for EF logging - will track NO. of records added
            //                    i++;
            //                }
            //                else
            //                {
            //                    Console.WriteLine($"\nSensor data is missing latitude or longitude. Skipped data.\n");
            //                }
            //            }
            //        }
            //    }
            //}

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
