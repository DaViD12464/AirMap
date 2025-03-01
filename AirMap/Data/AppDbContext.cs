namespace AirMap.Data
{
    using Microsoft.EntityFrameworkCore;
    using Newtonsoft.Json;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http;
    using System.Threading.Tasks;
    using Microsoft.Extensions.DependencyInjection;
    using System;
    using System.ComponentModel.DataAnnotations.Schema;

    public class AppDbContext : DbContext
    {
        private readonly HttpClient _httpClient;
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public AppDbContext(DbContextOptions<AppDbContext> options, HttpClient httpClient, IServiceScopeFactory serviceScopeFactory) : base(options)
        {
            _httpClient = httpClient;
            _serviceScopeFactory = serviceScopeFactory;
        }

        public DbSet<AirQualityReading> AirQualityReadings { get; set; } = null!;
        public DbSet<Source1Model> Source1Models { get; set; } = null!;
        public DbSet<Source2Model> Source2Models { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.EnableSensitiveDataLogging();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<AirQualityReading>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => new { e.Latitude, e.Longitude }).IsUnique();
            });

            modelBuilder.Entity<Source1Model>(entity =>
            {
                entity.HasKey(e => e.Device); // Assuming Device is unique and can be used as a primary key
            });

            modelBuilder.Entity<Source2Model>(entity =>
            {
                entity.HasKey(e => e.Id);
            });

            modelBuilder.Entity<Location>(entity =>
            {
                entity.HasKey(e => e.Id);
            });
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
                    var source2Models = JsonConvert.DeserializeObject<List<Source2Model>>(content);
                    if (source2Models != null)
                    {
                        using (var scope = _serviceScopeFactory.CreateScope())
                        {
                            var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                            foreach (var source2 in source2Models)
                            {
                                if (source2.Location != null)
                                {
                                    var airQualityReading = new AirQualityReading
                                    {
                                        Latitude = source2.Location.Latitude,
                                        Longitude = source2.Location.Longitude,
                                        PM1 = source2.GetSensorValue("P1"),
                                        PM25 = source2.GetSensorValue("P2"),
                                        PM10 = source2.GetSensorValue("P4"),
                                        Timestamp = source2.Timestamp
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
                    var source1Models = JsonConvert.DeserializeObject<List<Source1Model>>(content);
                    if (source1Models != null)
                    {
                        using (var scope = _serviceScopeFactory.CreateScope())
                        {
                            var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

                            foreach (var source1 in source1Models)
                            {
                                if (!string.IsNullOrEmpty(source1.Device))
                                {
                                    // Get existing entry, ensuring no tracking conflicts
                                    var existingSource1 = await dbContext.Source1Models
                                        .AsNoTracking() // Use AsNoTracking to avoid tracking the entity
                                        .FirstOrDefaultAsync(s => s.Device == source1.Device);

                                    if (existingSource1 != null)
                                    {
                                        // Detach the existing entity if it's already being tracked
                                        var trackedEntity = dbContext.Entry(existingSource1);
                                        if (trackedEntity.State == EntityState.Detached)
                                        {
                                            dbContext.Entry(existingSource1).State = EntityState.Detached;
                                        }

                                        // Now update the entity
                                        existingSource1.Timestamp = source1.Timestamp;
                                        existingSource1.Lat = source1.Lat;
                                        existingSource1.Lon = source1.Lon;
                                        dbContext.Update(existingSource1); // Update the entity in the context
                                    }
                                    else
                                    {
                                        // Add new entity if it doesn't exist
                                        dbContext.Source1Models.Add(source1);
                                    }

                                    // Insert new air quality reading
                                    var airQualityReading = new AirQualityReading
                                    {
                                        Latitude = source1.Lat,
                                        Longitude = source1.Lon,
                                        PM1 = string.IsNullOrWhiteSpace(source1.PM1) ? null : decimal.Parse(source1.PM1),
                                        PM25 = string.IsNullOrWhiteSpace(source1.PM25) ? null : decimal.Parse(source1.PM25),
                                        PM10 = string.IsNullOrWhiteSpace(source1.PM10) ? null : decimal.Parse(source1.PM10),
                                        Timestamp = source1.Epoch
                                    };
                                    dbContext.AirQualityReadings.Add(airQualityReading);
                                }
                                else
                                {
                                    Console.WriteLine("Invalid data: Device property is null or empty.");
                                }
                            }

                            // Save all changes
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
    }

    // Klasa reprezentująca dane w tabeli
    public class AirQualityReading
    {
        public int Id { get; set; }
        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }
        public decimal? PM1 { get; set; }
        public decimal? PM25 { get; set; }
        public decimal? PM10 { get; set; }
        public string? Name { get; set; }

        [Column(TypeName = "bit")]  // Specify correct SQL type
        public bool Indoor { get; set; }
        public string? Timestamp { get; set; }
        public decimal? Temperature { get; set; }
        public decimal? Humidity { get; set; }
        public decimal? HCHO { get; set; }
        public decimal? AveragePM1 { get; set; }
        public decimal? AveragePM25 { get; set; }
        public decimal? AveragePM10 { get; set; }
        public string? IJP { get; set; }
        public string? IJPString { get; set; }
        public string? IJPDescription { get; set; }
        public string? Color { get; set; }
    }


    // Modele dla danych z dwóch źródeł (uproszczone)
    public class Source1Model
    {
        public Source1Model()
        {
            Device = string.Empty; // Ensure Device is always initialized
        }
        public string? Timestamp { get; set; }
        public string Device { get; set; } = null!;
        public string? PM1 { get; set; }
        public string? PM25 { get; set; }
        public string? PM10 { get; set; }
        public string? Epoch { get; set; }
        public decimal Lat { get; set; }
        public decimal Lon { get; set; }
        public string? Name { get; set; }
        public string? Indoor { get; set; }
        public string? Temperature { get; set; }
        public string? Humidity { get; set; }
        public string? HCHO { get; set; }
        public string? AveragePM1 { get; set; }
        public string? AveragePM25 { get; set; }
        public string? AveragePM10 { get; set; }
        public string? IJPString { get; set; }
        public string? IJPDescription { get; set; }
        public string? Color { get; set; }
    }


    public class Source2Model
    {
        public long Id { get; set; }
        public string? Timestamp { get; set; }
        public Location? Location { get; set; }
        public List<SensorDataValue>? SensorDataValues { get; set; }

        public decimal? GetSensorValue(string valueType)
        {
            var value = SensorDataValues?.FirstOrDefault(v => v.ValueType == valueType)?.Value;
            return value != null ? decimal.Parse(value) : null;
        }
    }

    public class Location
    {
        public int Id { get; set; }
        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }
        public decimal Altitude { get; set; }
        public string? Country { get; set; }
        public int Indoor { get; set; }
    }

    public class SensorDataValue
    {
        public long Id { get; set; }
        public string? Value { get; set; }
        public string? ValueType { get; set; }
    }
}
