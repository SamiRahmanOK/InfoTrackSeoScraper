using InfoTrackSeoScraper.Main.Core.Interfaces;
using InfoTrackSeoScraper.Main.Core.Enums;
using InfoTrackSeoScraper.Main.Application.Services;
using InfoTrackSeoScraper.Main.Infrastructure.Data;
using InfoTrackSeoScraper.Main.Infrastructure.Services;
using InfoTrackSeoScraper.Main.Infrastructure.Policies;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Add this to ensure proper JSON serialization
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
    });

// Register database with cloud-compatible settings
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"), sqlServerOptions => 
    {
        sqlServerOptions.EnableRetryOnFailure(
            maxRetryCount: 5,
            maxRetryDelay: TimeSpan.FromSeconds(30),
            errorNumbersToAdd: null);
        sqlServerOptions.CommandTimeout(30);
    }));

// Register repositories
builder.Services.AddScoped<ISearchResultRepository, SearchResultRepository>();

// Register application services
builder.Services.AddScoped<IRankingService, RankingService>();

// Register HTTP clients with policies
builder.Services.AddHttpClient<GoogleScraperService>()
    .AddPolicyHandler(HttpClientPolicies.GetRetryPolicy());
builder.Services.AddHttpClient<BingScraperService>()
    .AddPolicyHandler(HttpClientPolicies.GetRetryPolicy());

// Register scrapers with keyed services
builder.Services.AddKeyedScoped<ISearchEngineScraperService, GoogleScraperService>(SearchEngineType.Google);
builder.Services.AddKeyedScoped<ISearchEngineScraperService, BingScraperService>(SearchEngineType.Bing);

// Add this line to register the adapter
builder.Services.AddScoped<IServiceProviderAdapter, ServiceProviderAdapter>();

// Register the factory that will handle the scraper service resolution
builder.Services.AddScoped<ISearchEngineFactory, SearchEngineFactory>();

// Enable CORS
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// Add standard logging
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();

var app = builder.Build();

// Apply migrations automatically during startup
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    try
    {
        dbContext.Database.Migrate();
    }
    catch (Exception ex)
    {
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while migrating the database.");
    }
}

if (!app.Environment.IsDevelopment())
{
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseCors();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller}/{action=Index}/{id?}");

app.MapFallbackToFile("index.html");

app.Run();
