using ValidationService;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Hosting;

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.UseUrls("http://0.0.0.0:5040");

var app = builder.Build();

// Health endpoint
app.MapGet("/health", () => Results.Ok(new { status = "validation service healthy" }));

// Updated validation endpoint matching BFF expectation
app.MapPost("/api/validation/validate", async (SubmitRequest request) =>
{
    // Replace this with your real validation logic
    Console.WriteLine($"Received validation request: {request}");

    // Example response
    var result = new ValidationResult(isValid: true, errors: Array.Empty<string>());
    return Results.Ok(result);
});

app.Run();

// --- Type definitions must go first ---
public record SubmitRequest(string Name, int Value, Dictionary<string, object> Metadata);
public record ValidationResult(bool isValid, string[] errors);