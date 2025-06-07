using AirMap.Models;

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
    public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
    {
        public DbSet<SensorModel> SensorModel { get; set; }
        public DbSet<Location> Location { get; set; }
        public DbSet<Sensor> Sensor { get; set; }
        public DbSet<SensorType> SensorType { get; set; }
        public DbSet<SensorDataValues> SensorDataValues { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.EnableSensitiveDataLogging(false).LogTo(Console.WriteLine, LogLevel.Warning); 
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            var entity = modelBuilder.Entity<SensorModel>();

            entity.ToTable("SensorModel");

            entity
                .HasOne(l => l.Location)
                .WithMany().HasForeignKey(l => l.LocationId);

            entity.OwnsOne(s => s.Sensor, sa =>
            {
                sa.WithOwner();
                sa.HasOne(s => s.SensorType);
                sa.Navigation(s=> s.SensorType).IsRequired(false);
            });
            entity.Navigation(e => e.Sensor).IsRequired(false);
                

            modelBuilder.Entity<SensorModel>(st =>
            {
                st.HasOne<SensorType>()
                    .WithMany()
                    .HasForeignKey("SensorTypeId");
            });

            entity.OwnsMany(sdv => sdv.SensorDataValues)
                .WithOwner().HasForeignKey("SensorDataValuesId");

            entity.Property(e => e.Device)
                .HasColumnType("varchar")
                .HasMaxLength(50);

            entity.Property(e => e.Pm1)
                .HasColumnType("decimal(18, 2)");

            entity.Property(e => e.Pm25)
                .HasColumnType("decimal(18, 2)");

            entity.Property(e => e.Pm10)
                .HasColumnType("decimal(18, 2)");

            entity.Property(e => e.Timestamp)
                .HasColumnType("datetime");

            entity.Property(e => e.Latitude)
                .HasColumnType("decimal(18, 8)");

            entity.Property(e => e.Longitude)
                .HasColumnType("decimal(18, 8)");

            entity.Property(e => e.Ijp)
                .HasColumnType("decimal(18, 8)");

            entity.Property(e => e.IjpStringEn)
                .HasColumnType("varchar")
                .HasMaxLength(255);

            entity.Property(e => e.IjpString)
                .HasColumnType("varchar")
                .HasMaxLength(255);

            entity.Property(e => e.IjpDescription)
                .HasColumnType("varchar")
                .HasMaxLength(512);

            entity.Property(e => e.IjpDescriptionEn)
                .HasColumnType("varchar")
                .HasMaxLength(512);

            entity.Property(e => e.Color)
                .HasColumnType("varchar")
                .HasMaxLength(20);

            entity.Property(e => e.Temperature)
                .HasColumnType("decimal(18, 2)");

            entity.Property(e => e.Humidity)
                .HasColumnType("decimal(18, 2)");

            entity.Property(e => e.AveragePm1)
                .HasColumnType("decimal(18, 2)");

            entity.Property(e => e.AveragePm25)
                .HasColumnType("decimal(18, 2)");

            entity.Property(e => e.AveragePm10)
                .HasColumnType("decimal(18, 2)");

            entity.Property(e => e.Name)
                .HasColumnType("varchar")
                .HasMaxLength(100);
            
            entity.Property(e=> e.Indoor)
                .HasColumnType("bit");

            entity.Property(e => e.PreviousIjp)
                .HasColumnType("varchar")
                .HasMaxLength(50);

            entity.Property(e => e.Hcho)
                .HasColumnType("decimal(18, 2)");

            entity.Property(e => e.AverageHcho)
                .HasColumnType("decimal(18, 2)");

            entity.Property(e => e.LocationName)
                .HasColumnType("varchar")
                .HasMaxLength(50);
                
            entity.HasKey(s => s.Id);
            entity.Property(p => p.Id).ValueGeneratedOnAdd();
            //entity.Property(p => p.Id).ValueGeneratedOnAdd(); -- need value to be generated on add for LookO2 // not necessarily for SensorModel
            entity.HasIndex(s => s.Id).IsUnique(); //ensure Id is unique
            entity.HasIndex(s => s.Device).IsUnique(); //ensure Device is unique

            entity.Property(e => e.SamplingRate)
                .HasColumnType("decimal(18, 2)");

        }
    }
    
}
