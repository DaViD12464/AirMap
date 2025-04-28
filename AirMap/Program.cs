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

// Rejestracja konfiguracji jako us�ugi
builder.Services.AddSingleton(configuration);

// Rejestracja HttpClient oraz serwis�w do uruchomienia w tle
builder.Services.AddHttpClient<AirQualityHostedService>(); // Rejestracja HttpClient dla HostedService
builder.Services.AddHostedService<AirQualityHostedService>(); // Rejestracja HostedService jako singleton
builder.Services.AddHostedService<SensorProcessor>(); // Rejestracja innego HostedService

//builder.Services.AddHttpClient<Fetcher>(); // Rejestracja Fetcher jako us�ugi    ------- unneccessary service?
//builder.Services.AddScoped<Fetcher>(); // Rejestracja Fetcher jako us�ugi o kr�tkim czasie �ycia ------- unneccessary service?


// Konfiguracja DbContext z u�yciem SQL Server
builder.Services.AddDbContext<AppDbContext>(
    options =>
    {
        options.UseSqlServer(
            builder.Configuration.GetConnectionString("DefaultConnection"),
            sqlServerOptions =>
            { // przy pierwszej pr�bie po��czenia z baz� danych baza time-outuje - pr�ba naprawienia poprzez retryonfailure
                sqlServerOptions.EnableRetryOnFailure(
                    maxRetryCount: 5,
                    maxRetryDelay: TimeSpan.FromSeconds(10),
                    errorNumbersToAdd: null
                    ); // Retry up to 5 times when connection times out
            }
        );
        options.LogTo(Console.WriteLine, LogLevel.Warning); // Log warnings to the console
    });




// Test po��czenia z baz� danych
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
    // Domy�lna warto�� HSTS: 30 dni. Mo�esz zmieni� to w scenariuszach produkcyjnych
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
