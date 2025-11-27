using Microsoft.AspNetCore.Http.HttpResults;
using StockSentiment.Core.Models;
using StockSentiment.Core.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Add services to the container.
builder.Services.AddOpenApi();

// Register Providers
builder.Services.AddHttpClient<IStockProvider, StockSentiment.Infrastructure.Services.AlphaVantageStockProvider>();
builder.Services.AddHttpClient<INewsProvider, StockSentiment.Infrastructure.Services.NewsApiProvider>();
builder.Services.AddSingleton<ISentimentAnalyzer, StockSentiment.Infrastructure.Services.SimpleSentimentAnalyzer>();

// Register Main Service
builder.Services.AddScoped<ISentimentService, RealSentimentService>();

// Add CORS for local development
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins("https://localhost:7076", "http://localhost:5232")
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseCors();
app.UseHttpsRedirection();

app.MapGet("/api/sentiment/{symbol}", async Task<Results<Ok<SentimentSnapshot>, BadRequest<string>>> (
    string symbol,
    ISentimentService sentimentService,
    CancellationToken cancellationToken) =>
{
    if (string.IsNullOrWhiteSpace(symbol))
    {
        return TypedResults.BadRequest("Symbol is required.");
    }

    var snapshot = await sentimentService.GetSentimentAsync(symbol, cancellationToken);
    return TypedResults.Ok(snapshot);
})
.WithName("GetSentiment")
.WithOpenApi();

app.Run();
