/*
using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using AirMap.Models;
using AirMap.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;


namespace AirMap.HostedServices
{
    public class Fetcher
    {
        private readonly HttpClient _httpClient;
        private readonly IServiceScopeFactory _scopeFactory;

        public Fetcher(HttpClient httpClient, IServiceScopeFactory scopeFactory)
        {
            _httpClient = httpClient;
            _scopeFactory = scopeFactory;
        }

        public async Task FetchAndSaveData(string url, string apiKey)
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
                Console.WriteLine($"Response content: {content}");

                if (url.Contains("looko2"))
                {
                    var root = JsonConvert.DeserializeObject<AirMap.Models.Root>(content);
                    if (root?.Sensors != null)
                    {
                        using (var scope = _scopeFactory.CreateScope())
                        {
                            var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                            foreach (var sensor in root.Sensors)
                            {
                                if (sensor.Lat.HasValue && sensor.Lon.HasValue)
                                {
                                    var Sensor1Models = new Source1Model
                                    {
                                        Timestamp = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                                        Device = sensor.Device ?? "Unknown_Device",
                                        PM1 = sensor.PM1.HasValue ? sensor.PM1.Value.ToString() : null,
                                        PM25 = sensor.PM25.HasValue ? sensor.PM25.Value.ToString() : null,
                                        PM10 = sensor.PM10.HasValue ? sensor.PM10.Value.ToString() : null,
                                        Epoch = sensor.Epoch?.ToString(),
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
                                    dbContext.Source1Models.Add(Sensor1Models);

                                    var airQualityReading = new AirQualityReading
                                    {
                                        Latitude = (decimal)sensor.Lat.Value,
                                        Longitude = (decimal)sensor.Lon.Value,
                                        PM1 = sensor.PM1.HasValue ? (decimal?)sensor.PM1.Value : null,
                                        PM25 = sensor.PM25.HasValue ? (decimal?)sensor.PM25.Value : null,
                                        PM10 = sensor.PM10.HasValue ? (decimal?)sensor.PM10.Value : null,
                                        Timestamp = sensor.Epoch?.ToString(),
                                       
                                    };

                                    dbContext.AirQualityReadings.Add(airQualityReading);
                                }
                            }


                            await dbContext.SaveChangesAsync();


                        }
                    }
                }

                else if (url.Contains("sensor.community"))
                {
                    var sensorSets = JsonConvert.DeserializeObject<List<AirMap.Models.SensorSet2>>(content);
                    if (sensorSets != null)
                    {
                        using (var scope = _scopeFactory.CreateScope())
                        {
                            var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                            foreach (var sensorSet in sensorSets)
                            {
                                if (sensorSet.location?.latitude != null && sensorSet.location?.longitude != null)
                                {
                                    var airQualityReading = new AirQualityReading
                                    {
                                        //Latitude = decimal.TryParse(sensorSet.location.latitude, out var lat) ? lat : (decimal?)null,
                                        //Longitude = decimal.TryParse(sensorSet.location.longitude, out var lon) ? lon : (decimal?)null,
                                        Timestamp = sensorSet.timestamp?.ToString(),
                                        PM1 = TryGetSensorValue(sensorSet.sensordatavalues, "P1"),
                                        PM25 = TryGetSensorValue(sensorSet.sensordatavalues, "P2"),
                                        PM10 = TryGetSensorValue(sensorSet.sensordatavalues, "P4")
                                    };


                                    dbContext.AirQualityReadings.Add(airQualityReading);
                                }
                            }

                            try
                            {
                                await dbContext.SaveChangesAsync();
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine("❌ Błąd podczas zapisu danych do bazy:");
                                Console.WriteLine(ex.Message);
                                // Tu możesz dodać np. logowanie do pliku albo dodatkowy retry
                            }

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

        private decimal? TryGetSensorValue(List<AirMap.Models.SensorDataValues>? values, string type)
        {
            var val = values?.FirstOrDefault(v => v.value_type == type)?.value;
            return decimal.TryParse(val, out var parsed) ? parsed : (decimal?)null;
        }
    }



}
*/