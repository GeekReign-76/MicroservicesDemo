using BffApi;
using BffApi.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;

var builder = WebApplication.CreateBuilder(args);

// Register typed HttpClients for services with BaseAddress
builder.Services.AddHttpClient<ValidationServiceClient>(client =>
{
    client.BaseAddress = new Uri("http://localhost:5040/"); //Validation service URL
});


builder.Services.AddHttpClient<ProcessingServiceClient>(client =>
{
    client.BaseAddress = new Uri("http://localhost:5035"); // Processing service URL
});

// CORS policy to allow React frontend at localhost:3000
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
        policy.WithOrigins("http://localhost:3000")
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
    var validation = await validationClient.Validate(req);
    if (!validation.IsValid)
        return Results.BadRequest(validation.Errors);

    var processed = await processingClient.Process(req);
    return Results.Ok(new { status = "success", record = processed });
});

app.Run();

// DTO for POST requests
public record SubmitRequest(string Name, int Value, object Metadata);
