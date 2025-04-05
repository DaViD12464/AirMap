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
    using System.ComponentModel.DataAnnotations;

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
            optionsBuilder.EnableSensitiveDataLogging(false).LogTo(Console.WriteLine, LogLevel.Warning); 
        }



        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AirQualityReading>()
                .Property(e => e.AveragePM1)
                .HasColumnType("decimal(18, 2)");

            modelBuilder.Entity<AirQualityReading>()
                .Property(e => e.AveragePM10)
                .HasColumnType("decimal(18, 2)");

            modelBuilder.Entity<AirQualityReading>()
                .Property(e => e.AveragePM25)
                .HasColumnType("decimal(18, 2)");

            modelBuilder.Entity<AirQualityReading>()
                .Property(e => e.HCHO)
                .HasColumnType("decimal(18, 2)");

            modelBuilder.Entity<AirQualityReading>()
                .Property(e => e.Humidity)
                .HasColumnType("decimal(18, 2)");

            modelBuilder.Entity<AirQualityReading>()
                .Property(e => e.Latitude)
                .HasColumnType("decimal(18, 8)");

            modelBuilder.Entity<AirQualityReading>()
                .Property(e => e.Longitude)
                .HasColumnType("decimal(18, 8)");

            modelBuilder.Entity<AirQualityReading>()
                .Property(e => e.PM1)
                .HasColumnType("decimal(18, 2)");

            modelBuilder.Entity<AirQualityReading>()
                .Property(e => e.PM10)
                .HasColumnType("decimal(18, 2)");

            modelBuilder.Entity<AirQualityReading>()
                .Property(e => e.PM25)
                .HasColumnType("decimal(18, 2)");

            modelBuilder.Entity<AirQualityReading>()
                .Property(e => e.Temperature)
                .HasColumnType("decimal(18, 2)");

            modelBuilder.Entity<Location>()
                .Property(e => e.Altitude)
                .HasColumnType("decimal(18, 2)");

            modelBuilder.Entity<Location>()
                .Property(e => e.Latitude)
                .HasColumnType("decimal(18, 8)");

            modelBuilder.Entity<Location>()
                .Property(e => e.Longitude)
                .HasColumnType("decimal(18, 8)");

            modelBuilder.Entity<Source1Model>().HasKey(s => s.Id);
            modelBuilder.Entity<Source1Model>().Property(p => p.Id).ValueGeneratedOnAdd();

            modelBuilder.Entity<Source1Model>().HasIndex(s => s.Device).IsUnique();

            modelBuilder.Entity<Source1Model>()
                .Property(e => e.Lat)
                .HasColumnType("decimal(18, 8)");

            modelBuilder.Entity<Source1Model>()
                .Property(e => e.Lon)
                .HasColumnType("decimal(18, 8)");

        }

        private decimal? TryGetSensorValue(List<AirMap.Models.SensorDataValues>? values, string type)
        {
            var val = values?.FirstOrDefault(v => v.value_type == type)?.value;
            return decimal.TryParse(val, out var parsed) ? parsed : (decimal?)null;
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
                        using (var scope = _serviceScopeFactory.CreateScope())
                        {
                            var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                            foreach (var sensor in root.Sensors)
                            {
                                if (sensor.Lat.HasValue && sensor.Lon.HasValue)
                                {
                                    var airQualityReading = new AirQualityReading
                                    {
                                        Latitude = (decimal)sensor.Lat.Value,
                                        Longitude = (decimal)sensor.Lon.Value,
                                        PM1 = sensor.PM1.HasValue ? (decimal?)sensor.PM1.Value : null,
                                        PM25 = sensor.PM25.HasValue ? (decimal?)sensor.PM25.Value : null,
                                        PM10 = sensor.PM10.HasValue ? (decimal?)sensor.PM10.Value : null,
                                        Timestamp = sensor.Epoch?.ToString()
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
                        using (var scope = _serviceScopeFactory.CreateScope())
                        {
                            var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                            foreach (var sensorSet in sensorSets)
                            {
                                if (sensorSet.location?.latitude != null && sensorSet.location?.longitude != null)
                                {
                                    var airQualityReading = new AirQualityReading
                                    {
                                        Latitude = decimal.Parse(sensorSet.location.latitude),
                                        Longitude = decimal.Parse(sensorSet.location.longitude),
                                        Timestamp = sensorSet.timestamp?.ToString(),
                                        PM1 = TryGetSensorValue(sensorSet.sensordatavalues, "P1"),
                                        PM25 = TryGetSensorValue(sensorSet.sensordatavalues, "P2"),
                                        PM10 = TryGetSensorValue(sensorSet.sensordatavalues, "P4")
                                    };

                                    await dbContext.Database.ExecuteSqlRawAsync("SET IDENTITY_INSERT Source1Models ON");

                                    dbContext.AirQualityReadings.Add(airQualityReading);
                                }
                            }

                            try
                            {
                                await dbContext.Database.ExecuteSqlRawAsync("SET IDENTITY_INSERT Source1Models OFF");
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
    }
    
    // Klasa reprezentująca dane w tabeli
    public class AirQualityReading
    {
        [Key]
        public long Id { get; set; }
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

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }
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
        public long Id { get; set; }
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
    