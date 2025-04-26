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
        public DbSet<AirQualityReading> AirQualityReadings { get; set; } = null!;
        public DbSet<Source1Model> Source1Models { get; set; } = null!;
        public DbSet<Source2Model> Source2Models { get; set; } = null!;
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)       {    }

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

        [Column(TypeName = "bit")] 
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


    // Modele dla danych 
    public class Source1Model //LookO2
    {
        public Source1Model()
        {
            Device = string.Empty;
        }
        [JsonIgnore]
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }
        public string? Timestamp { get; set; }
        public string Device { get; set; } = null!;
        public string? PM1 { get; set; }
        public string? PM25 { get; set; }
        public string? PM10 { get; set; }
        public string? Epoch { get; set; } // epoch is datetime value in UNIX -- value during fetching data converted to DateTime
        public decimal? Lat { get; set; }
        public decimal? Lon { get; set; }
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


    public class Source2Model //sensor community
    {
        public long Id { get; set; }
        public string? Timestamp { get; set; }
        public Location? Location { get; set; }
        public List<SensorDataValue>? SensorDataValues { get; set; }

        public decimal? GetSensorValue(string valueType)
        {
            var value = SensorDataValues?.FirstOrDefault(v => v.ValueType == valueType)?.Value;
            if (decimal.TryParse(value, out var result)) return result; return null;

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
    