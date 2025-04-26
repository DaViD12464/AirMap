using AirMap.Controllers;
using AirMap.Models;
using Microsoft.EntityFrameworkCore;
using AirMap.Data;
// using AirMap.HostedServices; ------- unneccessary service?

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

//builder.Services.AddHttpClient<Fetcher>(); // Rejestracja Fetcher jako us³ugi    ------- unneccessary service?
//builder.Services.AddScoped<Fetcher>(); // Rejestracja Fetcher jako us³ugi o krótkim czasie ¿ycia ------- unneccessary service?


// Konfiguracja DbContext z u¿yciem SQL Server
builder.Services.AddDbContext<AppDbContext>(
    options =>
    {
        options.UseSqlServer(
            builder.Configuration.GetConnectionString("DefaultConnection"),
            sqlServerOptions =>
            { // przy pierwszej próbie po³¹czenia z baz¹ danych baza time-outuje - próba naprawienia poprzez retryonfailure
                sqlServerOptions.EnableRetryOnFailure(5); // Retry up to 5 times when connection times out
                sqlServerOptions.CommandTimeout(60); // Retry timeout as 60 seconds
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

app.UseAuthorization();

app.UseAuthentication();


app.MapHealthChecks("/health");



app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
