using System.Net.NetworkInformation;
using AirMap.Controllers;
using AirMap.Data;
using AirMap.Models;
using Microsoft.EntityFrameworkCore;

try
{
    var builder = WebApplication.CreateBuilder(args);

    var configuration = builder.Configuration;

// Add services to the container.
    builder.Services.AddControllersWithViews();
    builder.Services.Configure<SocialMediaLinks>(builder.Configuration.GetSection("SocialMediaLinks"));

// Rejestracja konfiguracji jako us³ugi
    builder.Services.AddSingleton(configuration);

// Rejestracja HttpClient oraz serwisów do uruchomienia w tle
    builder.Services.AddHttpClient<AirQualityHostedService>(); // Rejestracja HttpClient dla HostedService
    builder.Services.AddHostedService<AirQualityHostedService>(); // Rejestracja HostedService jako singleton
    builder.Services.AddHostedService<SensorProcessor>(); // Rejestracja innego HostedService


// Konfiguracja DbContext z u¿yciem SQL Server
    builder.Services.AddDbContext<AppDbContext>(options =>
    {
        options.UseSqlServer(
            builder.Configuration.GetConnectionString("DefaultConnection"),
            sqlServerOptions =>
            {
                // przy pierwszej próbie po³¹czenia z baz¹ danych baza time-outuje - próba naprawienia poprzez retryonfailure
                sqlServerOptions.EnableRetryOnFailure(
                    maxRetryCount: 10,
                    maxRetryDelay: TimeSpan.FromSeconds(30),
                    errorNumbersToAdd: null
                );
            }
        );
        options.LogTo(Console.WriteLine, LogLevel.Warning); // Log warnings to the console
    });




// Test po³¹czenia z baz¹ danych
    var ConStr = builder.Configuration.GetConnectionString("DefaultConnection");
    if (string.IsNullOrEmpty(ConStr))
    {
        throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

    }

    builder.Services.AddHealthChecks()
        .AddDbContextCheck<AppDbContext>();

    var app = builder.Build();

// Konfiguracja potoku HTTP
    if (!app.Environment.IsDevelopment())
    {
        app.UseExceptionHandler("/Home/Error");
        // Domyœlna wartoœæ HSTS: 30 dni. Mo¿esz zmieniæ to w scenariuszach produkcyjnych
        app.UseHsts();
    }

    app.UseHttpsRedirection();
    app.UseStaticFiles();

    app.UseRouting();

    app.UseAuthentication();
    app.UseAuthorization();


    app.MapHealthChecks("/healthz");



    app.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}");

    app.Run();
}
catch (Exception ex)
{
    Console.WriteLine($"An error occurred: {ex.Message}");
}