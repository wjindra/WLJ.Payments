using Microsoft.EntityFrameworkCore;
using WLJ.Payments.Migrations;
using WLJ.Payments.Migrations.Models;

var builder = WebApplication.CreateBuilder(args);

// Add service defaults & Aspire client integrations.
builder.AddServiceDefaults();
builder.AddRedisClientBuilder("cache")
    .WithOutputCache();

builder.AddNpgsqlDbContext<PaymentsDbContext>("paymentsdb");

// Add services to the container.
builder.Services.AddProblemDetails();

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseOutputCache();

if (app.Environment.IsDevelopment())
{
    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<PaymentsDbContext>();
    await db.Database.EnsureCreatedAsync();
}

string[] summaries = ["Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"];

var api = app.MapGroup("/api");
api.MapGet("weatherforecast", async (PaymentsDbContext db) =>
{
    var forecasts = Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        {
            Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            TemperatureC = Random.Shared.Next(-20, 55),
            Summary = summaries[Random.Shared.Next(summaries.Length)]
        })
        .ToList();

    db.WeatherForecasts.AddRange(forecasts);
    await db.SaveChangesAsync();

    return forecasts;
})
.CacheOutput(p => p.Expire(TimeSpan.FromSeconds(5)))
.WithName("GetWeatherForecast");

api.MapGet("weatherforecast/history", async (PaymentsDbContext db) =>
{
    return await db.WeatherForecasts
        .OrderByDescending(w => w.Id)
        .Take(20)
        .ToListAsync();
})
.WithName("GetWeatherHistory");

app.MapDefaultEndpoints();

app.UseFileServer();

app.Run();
