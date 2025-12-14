using ValidationService;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Hosting;

var builder = WebApplication.CreateBuilder(args);

// Configure Kestrel URL
builder.WebHost.UseUrls("http://0.0.0.0:5040");

var app = builder.Build();

// Health endpoint
app.MapGet("/health", () => Results.Ok(new { status = "validation service healthy" }));

// Example validation endpoint (async)
app.MapPost("/validation-service", async (object input) =>
{
    // Simulate async processing or DB call
    await Task.Delay(10); // Dummy async operation

    // Simple dummy validation
    return Results.Ok(new { isValid = true, errors = Array.Empty<string>() });
});

app.Run();
