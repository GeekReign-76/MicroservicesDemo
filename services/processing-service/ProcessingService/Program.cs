using ProcessingService;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using System;
using Npgsql; // Ensure Npgsql is added via NuGet

var builder = WebApplication.CreateBuilder(args);

// 🔑 Ensure environment variables are loaded
builder.Configuration.AddEnvironmentVariables();

// ✅ Read connection string from environment variables
var connectionString = builder.Configuration["ConnectionStrings:DefaultConnection"];
if (string.IsNullOrWhiteSpace(connectionString))
{
    throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
}

// Try to verify DB connection at startup
try
{
    using var testConn = new NpgsqlConnection(connectionString);
    testConn.Open(); // Will throw if connection fails
    Console.WriteLine("Processing service: database connection successful");
}
catch (Exception ex)
{
    Console.WriteLine($"Processing service: database connection FAILED -> {ex.Message}");
    throw; // Optional: stop the service if DB connection is required
}

// Configure Kestrel URL
builder.WebHost.UseUrls("http://0.0.0.0:5035");

var app = builder.Build();

// Health endpoint
app.MapGet("/health", () => Results.Ok(new { status = "processing service healthy" }));

// Optional DB health check endpoint
app.MapGet("/db-health", async () =>
{
    try
    {
        using var conn = new NpgsqlConnection(connectionString);
        await conn.OpenAsync();
        return Results.Ok(new { db = "connected" });
    }
    catch (Exception ex)
    {
        return Results.Problem(ex.Message);
    }
});

// Example processing endpoint
app.MapPost("/processing-service", async (object input) =>
{
    // Your processing logic here
    return Results.Ok(new { processed = true, input });
});

app.Run();
