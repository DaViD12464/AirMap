using AirMap.Controllers;
using AirMap.Models;
using Microsoft.EntityFrameworkCore;
using AirMap.Data;

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
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))); // Konfiguracja DbContext z SQL Server


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

app.UseAuthorization();

// Test po³¹czenia z baz¹ danych
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    try
    {
        Console.WriteLine("Sprawdzanie po³¹czenia z baz¹ danych...");
        dbContext.Database.OpenConnection(); // Otwórz po³¹czenie z baz¹ danych
        Console.WriteLine("Po³¹czenie z baz¹ danych zosta³o nawi¹zane pomyœlnie!");
        dbContext.Database.CloseConnection(); // Zamknij po³¹czenie (opcjonalne)
    }
    catch (Exception ex)
    {
        Console.WriteLine($"B³¹d po³¹czenia z baz¹ danych: {ex.Message}");
    }
}

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
