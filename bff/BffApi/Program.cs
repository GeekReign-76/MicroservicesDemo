using BffApi;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;

var builder = WebApplication.CreateBuilder(args);

//
// 🔗 HttpClients — container-to-container networking
//
builder.Services.AddHttpClient<ValidationServiceClient>(client =>
{
    client.BaseAddress = new Uri("http://validation:5040/");
});

builder.Services.AddHttpClient<ProcessingServiceClient>(client =>
{
    client.BaseAddress = new Uri("http://processing:5035/");
});

//
// 🌍 CORS — relaxed for local testing
//
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod());
});

// 🌐 Bind to all interfaces for Docker
builder.WebHost.UseUrls("http://0.0.0.0:4000");

var app = builder.Build();

app.UseCors("AllowFrontend");

//
// 🚀 Main BFF endpoint
//
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

//
// ❤️ Health check
//
app.MapGet("/api/health", () =>
    Results.Ok(new { status = "bff service healthy" })
);

//
// 🔎 DEBUG — BFF → Validation connectivity
//
app.MapGet("/_debug/validation", async (ValidationServiceClient validationClient) =>
{
    var dummyRequest = new SubmitRequest("Test", 0, new { });
    var result = await validationClient.Validate(dummyRequest);

    return Results.Ok(new
    {
        from = "validation",
        isValid = result.IsValid,
        errors = result.Errors
    });
});

app.Run();

//
// DTO
//
public record SubmitRequest(string Name, int Value, object Metadata);
