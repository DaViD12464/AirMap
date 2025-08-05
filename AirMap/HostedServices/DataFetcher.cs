using AirMap.Data;
using AirMap.DTOs;
using AirMap.Helper;
using AirMap.Helpers;
using Newtonsoft.Json;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using static System.Formats.Asn1.AsnWriter;
using AirMap.Models;
using Location = AirMap.Models.Location;
using Sensor = AirMap.Models.Sensor;
using SensorDataValues = AirMap.Models.SensorDataValues;
using SensorType = AirMap.Models.SensorType;

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
        try
        {
            _timer = new Timer(async state => await DoWorkAsync(state), null, TimeSpan.Zero, TimeSpan.FromMinutes(60));
        }
        catch
        {
            StartAsync(CancellationToken.None);
        }
        return Task.CompletedTask;
    }

    private async Task DoWorkAsync(object? state)
    {
        await _semaphore.WaitAsync();
        try
        {
            string? look02ApiString = _configuration["Look02ApiString"];
            string? lookO2ApiKey = _configuration["LookO2ApiKey"];
            if (!string.IsNullOrEmpty(look02ApiString) && !string.IsNullOrEmpty(lookO2ApiKey))
            {
                Console.WriteLine("\n---------------------------");
                Console.WriteLine($"LookO2 API string: {look02ApiString + lookO2ApiKey}");
                Console.WriteLine("---------------------------");
                // Pass the required parameters to FetchAndSaveData
                await FetchAndSaveData(look02ApiString, lookO2ApiKey);
            }
            else
            {
                Console.WriteLine("\n---------------------------");
                Console.WriteLine("LookO2 API string / key is not available.");
                Console.WriteLine("---------------------------");
            }

            // Fetch data from Sensor.Community and save to DB
            string? sensorCommunityApiString = _configuration["SensorCommunityApiString"];
            if (!string.IsNullOrEmpty(sensorCommunityApiString))
            {
                Console.WriteLine("\n---------------------------");
                Console.WriteLine($"SensorCommunity API string: {sensorCommunityApiString}");
                Console.WriteLine("---------------------------");
                // Pass the required parameters to FetchAndSaveData
                await FetchAndSaveData(sensorCommunityApiString, string.Empty);
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
        string requestUrl = url;
        // If the API key needs to be included as a query parameter, use UriBuilder
        // For demonstration, let's assume the API key should be added as "api_key" query parameter if required
        if (!string.IsNullOrEmpty(apiKey) && url.Contains("api_key=") == false)
        {
            var uriBuilder = new UriBuilder(url);
            var query = System.Web.HttpUtility.ParseQueryString(uriBuilder.Query);
            query["api_key"] = apiKey;
            uriBuilder.Query = query.ToString();
            requestUrl = uriBuilder.ToString();
        }
        using var request = new HttpRequestMessage(HttpMethod.Get, requestUrl);
        if (!string.IsNullOrEmpty(apiKey))
        {
            request.Headers.Add("X-Api-Key", apiKey);
        }

        var response = await _httpClient.SendAsync(request);
        if (response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync();

            #region LookO2

            if (url.Contains("looko2"))
            {
                Console.WriteLine("\n\nAdding LookO2 data to DB...\n\n");
                var lookO2Data = JsonConvert.DeserializeObject<List<LookO2Dto>>(content);
                var lookO2DataModel = lookO2Data!.Select(SensorModel.FromDto).ToHashSet();

                lookO2Data?.Clear();
                lookO2Data = null;

                // Create Database Scope
                using var scope = _serviceScopeFactory.CreateScope();
                var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

                // Filter out duplicates and null/empty Device values
                var filteredModels = lookO2DataModel?
                    .Where(m => !string.IsNullOrEmpty(m.Device)) // Exclude null/empty Device
                    .Where(m => !m.Latitude.Equals(0.0) || !m.Longitude.Equals(0.0)) //ensure devices with Lat == 0 and Lon == 0 are not included
                    .GroupBy(m => m.Device) // Group by Device
                    .SelectMany(g => g)
                    .ToHashSet();

                lookO2DataModel?.Clear();
                lookO2DataModel = null;

                // Get Dictionary map to filter changes:
                var existingModels = DatabaseHelper.GetAll<SensorModel>(dbContext)
                    .Where(m => !string.IsNullOrEmpty(m.Device))
                    .ToDictionary(m => m.Device!, m => m);

                // Then filter both new and changed devices
                var newOrUpdatedModels = filteredModels!
                    .Where(m =>
                    {
                        if (m.Device == null) return false;
                        if (!existingModels.TryGetValue(m.Device, out var existing)) return true;

                        return DataFetcherHelper.AreNotEqual(m.Pm1, existing.Pm1) ||
                               DataFetcherHelper.AreNotEqual(m.Pm25, existing.Pm25) ||
                               DataFetcherHelper.AreNotEqual(m.Pm10, existing.Pm10) ||
                               DataFetcherHelper.AreNotEqual(m.Temperature, existing.Temperature) ||
                               DataFetcherHelper.AreNotEqual(m.Humidity, existing.Humidity) ||
                               DataFetcherHelper.AreNotEqual(m.Ijp, existing.Ijp);
                    })
                    .ToHashSet();


                if (filteredModels != null)
                {
                    var i = 1;
                    foreach (var sensor in newOrUpdatedModels)
                    {
                        if (!sensor.Latitude.Equals(0.0) || !sensor.Longitude.Equals(0.0))
                        {
                            var existingSensor = await dbContext.SensorModel.FirstOrDefaultAsync(s => s.Device == sensor.Device);
                            var addOrUpdate = "";

                            if (existingSensor != null)
                            {
                                existingSensor.Pm1 = sensor.Pm1;
                                existingSensor.Pm25 = sensor.Pm25;
                                existingSensor.Pm10 = sensor.Pm10;
                                existingSensor.Temperature = sensor.Temperature;
                                existingSensor.Humidity = sensor.Humidity;
                                existingSensor.Ijp = sensor.Ijp;
                                existingSensor.Latitude = sensor.Latitude;
                                existingSensor.Longitude = sensor.Longitude;
                                dbContext.SensorModel.Update(existingSensor);
                                addOrUpdate = "U:";
                            }
                            else
                            {
                                dbContext.SensorModel.Add(sensor);
                                addOrUpdate = "A:";
                            }

                            Console.Write(" " + addOrUpdate + i + " ");
                            i++;
                        }
                        else
                        {
                            Console.WriteLine($"\nSensor data is missing latitude or longitude. Skipped data.\n");
                        }
                    }

                    try
                    {
                        Console.WriteLine("\nAdded to dbContext, ready to save to DB");
                        await dbContext.SaveChangesAsync();
                        Console.WriteLine("\nAdding to database finished.");
                    }
                    catch (DbUpdateException ex)
                    {
                        Console.WriteLine(ex.InnerException?.Message ?? ex.Message);
                        Environment.Exit(1);
                    }
                    finally
                    {
                        filteredModels?.Clear();
                        existingModels?.Clear();
                        newOrUpdatedModels?.Clear();

                    }
                }
                else
                    Console.WriteLine("\nSource1Data is empty.\n");
            }

            #endregion

            #region SensorCommunity

            if (url.Contains("sensor.community"))
            {
                Console.WriteLine("\n\nAdding sensor.community data to DB...\n\n");
                var scData = JsonConvert.DeserializeObject<List<SensorCommunityDto>>(content);
                var scDataModel = scData!.Select(SensorModel.FromDto).ToHashSet();

                using var scope = _serviceScopeFactory.CreateScope();
                var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

                var filteredModels = scDataModel?
                    .Where(m => m is { Location: not null, SensorDataValues: not null })
                    .Where(m => m.Location!.Latitude != 0 || m.Location!.Longitude != 0)
                    .GroupBy(m =>
                        $"{m.Sensor!.Pin}_{m.Location!.Latitude}_{m.Location!.Longitude}") // Unique by Pin + Lat/Lon
                    .Select(g => g.First())
                    .ToHashSet();

                // Check existing devices in DB (might want a better uniqueness check - currently as combination of pin_lat_lon instead of Id)
                var existingModels = DatabaseHelper.GetAll<SensorModel>(dbContext)
                    .Where(m => m is { Sensor: not null, Location: not null })
                    .ToDictionary(m => m.Sensor?.Pin, m => m);

                // Then filter both new and changed devices
                var newOrUpdatedModels = filteredModels!
                    .Where(m =>
                    {
                        var pin = m.Sensor?.Pin;
                        if (pin == null) return false;
                        if (!existingModels.TryGetValue(pin, out var existing)) return true;

                        var currentDict = m.SensorDataValues!
                            .GroupBy(v => v.ValueType.ToLowerInvariant())
                            .ToDictionary(g => g.Key, g => g.FirstOrDefault()?.Value);

                        var existingDict = existing.SensorDataValues!
                            .GroupBy(v => v.ValueType.ToLowerInvariant())
                            .ToDictionary(g => g.Key, g => g.FirstOrDefault()?.Value);

                        // Build a union of all keys (value types)
                        var allValueTypes = new HashSet<string>(currentDict.Keys);
                        allValueTypes.UnionWith(existingDict.Keys);

                        foreach (var valueType in allValueTypes)
                        {
                            currentDict.TryGetValue(valueType, out var currentValue);
                            existingDict.TryGetValue(valueType, out var existingValue);

                            if (DataFetcherHelper.AreNotEqual(currentValue, existingValue))
                                return true;
                        }

                        return false;
                    })
                    .ToHashSet();


                if (scDataModel != null)
                {
#if DEBUG
                    var i = 1;
#endif
                    foreach (var sensor in newOrUpdatedModels)
                    {
                        if (sensor.Location?.Latitude != null && sensor.Location?.Longitude != null)
                        {
                            var existingSensor = await dbContext.SensorModel.FirstOrDefaultAsync(s => s.Sensor!.Pin.Equals(sensor.Sensor!.Pin));
#if DEBUG
                            var addOrUpdate = "";
#endif
                            if (existingSensor != null)
                            {
                                existingSensor.SensorDataValues = sensor.SensorDataValues;
                                dbContext.SensorModel.Update(existingSensor);
#if DEBUG
                                addOrUpdate = "U:";
#endif
                            }
                            else
                            {
                                dbContext.SensorModel.Add(sensor);
#if DEBUG
                                addOrUpdate = "A:";
#endif
                            }

#if DEBUG
                            Console.Write(" " + addOrUpdate + i +
                                          " "); //Added as replacement for EF logging - will track NO. of records added
                            i++;
#endif
                        }
                        else
                        {
                            Console.WriteLine($"\nSensor data is missing latitude or longitude. Skipped data.\n");
                        }
                    }

                    try
                    {
                        Console.WriteLine("\nAdded to dbContext, ready to save to DB");
                        await dbContext.SaveChangesAsync();
                        Console.WriteLine("\nAdding to database finished.");
                    }
                    catch (DbUpdateException ex)
                    {
                        Console.WriteLine(ex.InnerException?.Message ?? ex.Message);
                        Environment.Exit(1);
                    }
                    finally
                    {
                        filteredModels?.Clear();
                        filteredModels = null;
                        existingModels?.Clear();
                        existingModels = null;
                        newOrUpdatedModels?.Clear();
                        newOrUpdatedModels = null;
                    }
                }
            }

            content = null;
        }
        else
        {
            Console.WriteLine($"\nFailed to fetch data from {url}\n");
            Console.WriteLine($"\nStatus code: {response.StatusCode}\n");
            Console.WriteLine($"\nReason: {response.ReasonPhrase} \n");
        }


        #endregion

        GC.Collect();
        GC.WaitForPendingFinalizers();
        GC.Collect();
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
