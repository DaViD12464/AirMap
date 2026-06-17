using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace AirMap.Data
{
    public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
    {
        public AppDbContext CreateDbContext(string[] args)
        {   
            var basePath = Directory.GetCurrentDirectory();
            var configPath = Path.Combine(basePath, "appsettings.json");
            Console.WriteLine($"CurrentDirectory: {basePath}");
            Console.WriteLine($"appsettings.json exists: {File.Exists(configPath)}");

            var configuration = new ConfigurationBuilder()
                .SetBasePath(basePath)
                .AddJsonFile("appsettings.json", optional: false)
                .Build();
            var connectionString = configuration.GetConnectionString("DefaultConnection");
            if (string.IsNullOrEmpty(connectionString))
                throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

            var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();

            //REDUNDANT - migration from SQL Server / PostgreSQL to MySQL / MariaDB
            // optionsBuilder.UseSqlServer(connectionString);

            optionsBuilder.UseMySql(
                connectionString,
                new MariaDbServerVersion(new Version(10, 5, 29))
            );

            return new AppDbContext(optionsBuilder.Options);
        }
    }
}
