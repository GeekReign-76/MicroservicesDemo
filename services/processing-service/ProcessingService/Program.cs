using ProcessingService;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using System;
using Npgsql;

var builder = WebApplication.CreateBuilder(args);

// Load environment variables
builder.Configuration.AddEnvironmentVariables();

// Read connection string
var connectionString = builder.Configuration["ConnectionStrings:DefaultConnection"];
if (string.IsNullOrWhiteSpace(connectionString))
{
    throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
}

// Test DB connection asynchronously at startup
try
{
    await using var testConn = new NpgsqlConnection(connectionString);
    await testConn.OpenAsync();
    Console.WriteLine("Processing service: database connection successful");
}
catch (Exception ex)
{
    Console.WriteLine($"Processing service: database connection FAILED -> {ex.Message}");
    throw;
}

// Configure Kestrel URL
builder.WebHost.UseUrls("http://0.0.0.0:5035");

var app = builder.Build();

// Health endpoint
app.MapGet("/health", () => Results.Ok(new { status = "processing service healthy" }));

// DB health check endpoint (async)
app.MapGet("/db-health", async () =>
{
    try
    {
        await using var conn = new NpgsqlConnection(connectionString);
        await conn.OpenAsync();
        return Results.Ok(new { db = "connected" });
    }
    catch (Exception ex)
    {
        return Results.Problem(ex.Message);
    }
});

// Example processing endpoint (async)
app.MapPost("/processing-service", async (object input) =>
{
    // Simulate some async DB call
    await using var conn = new NpgsqlConnection(connectionString);
    await conn.OpenAsync();
    await using var cmd = new NpgsqlCommand("SELECT 1", conn);
    await cmd.ExecuteScalarAsync();

    return Results.Ok(new { processed = true, input });
});

app.Run();
