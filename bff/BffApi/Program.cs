// bff/BffApi/Program.cs
using BffApi;
using BffApi.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;

var builder = WebApplication.CreateBuilder(args);

// Register typed HttpClients for other services using Kubernetes service DNS
builder.Services.AddHttpClient<ValidationServiceClient>(client =>
{
    client.BaseAddress = new Uri("http://validation-service:5040/"); // Kubernetes service name
});

builder.Services.AddHttpClient<ProcessingServiceClient>(client =>
{
    client.BaseAddress = new Uri("http://processing-service:5035/"); // Kubernetes service name
});

// Allow CORS from frontend NodePort (development only)
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
        policy.WithOrigins("http://localhost:30001") // NodePort for frontend
              .AllowAnyHeader()
              .AllowAnyMethod());
});

var app = builder.Build();

// Enable CORS
app.UseCors("AllowFrontend");

// Minimal API POST endpoint
app.MapPost("/api/submit", async (
    SubmitRequest req,
    ValidationServiceClient validationClient,
    ProcessingServiceClient processingClient) =>
{
    // Validate
    var validation = await validationClient.Validate(req);
    if (!validation.IsValid)
        return Results.BadRequest(validation.Errors);

    // Process
    var processed = await processingClient.Process(req);

    // Return result
    return Results.Ok(new { status = "success", record = processed });
});

app.Run();

// DTO for incoming requests
public record SubmitRequest(string Name, int Value, object Metadata);
