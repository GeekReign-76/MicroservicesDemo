using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using ProcessingService.Data;
using Microsoft.Extensions.Configuration;

// Create builder
var builder = WebApplication.CreateBuilder(args);

// Environment info
var env = builder.Environment.EnvironmentName;
Console.WriteLine($"Environment: {env}");
Console.WriteLine($"Working Directory: {Directory.GetCurrentDirectory()}");

// Dynamically resolve project root for configuration
var projectRoot = Directory.GetCurrentDirectory();

// Ensure appsettings.json exists
if (!File.Exists(Path.Combine(projectRoot, "appsettings.json")))
{
    // fallback if running from a subfolder like bin/Debug/net9.0
    projectRoot = Path.Combine(projectRoot, "..", "..", "..");
    projectRoot = Path.GetFullPath(projectRoot);
}

Console.WriteLine($"Using configuration path: {projectRoot}");

// Build configuration
var configuration = new ConfigurationBuilder()
.SetBasePath(projectRoot)
.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
.AddJsonFile($"appsettings.{env}.json", optional: true, reloadOnChange: true)
.AddEnvironmentVariables()
.Build();

// Read connection string
var connectionString = configuration.GetConnectionString("DefaultConnection");
Console.WriteLine($"Runtime ConnectionString: {connectionString}");
if (string.IsNullOrEmpty(connectionString))
{
    throw new InvalidOperationException($"Connection string 'DefaultConnection' not found in {projectRoot}");
}

// Register DbContext
builder.Services.AddDbContext<DataContext>(options => options.UseNpgsql(connectionString, opts => opts.EnableRetryOnFailure()));

// Add controllers
builder.Services.AddControllers();

// Add Swagger (OpenAPI)
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "ProcessingService API",
        Version = "v1"
    });
});

// Build app
var app = builder.Build();

// Enable Swagger only in development
if (builder.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "ProcessingService API V1");
        c.RoutePrefix = string.Empty; // Swagger at root /
    });
}

// Map controllers
app.MapControllers();

// Run the app
app.Run();
