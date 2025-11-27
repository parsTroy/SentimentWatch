using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Configuration;
using StockSentiment.Core.Models;
using StockSentiment.Core.Services;
using System.Net.Http.Json;

namespace StockSentiment.Infrastructure.Services;

public class AlphaVantageStockProvider : IStockProvider
{
    private readonly HttpClient _httpClient;
    private readonly string _apiKey;

    public AlphaVantageStockProvider(HttpClient httpClient, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _apiKey = configuration["StockData:ApiKey"] ?? throw new InvalidOperationException("StockData:ApiKey is missing");
    }

    public async Task<IReadOnlyList<PricePoint>> GetPriceHistoryAsync(string symbol, CancellationToken cancellationToken = default)
    {
        var url = $"https://www.alphavantage.co/query?function=TIME_SERIES_DAILY&symbol={symbol}&apikey={_apiKey}";
        var response = await _httpClient.GetFromJsonAsync<AlphaVantageResponse>(url, cancellationToken);

        if (response?.TimeSeries == null)
        {
            return Array.Empty<PricePoint>();
        }

        return response.TimeSeries
            .Select(kvp =>
            {
                if (DateTimeOffset.TryParse(kvp.Key, out var date) &&
                    decimal.TryParse(kvp.Value.Close, out var close))
                {
                    return new PricePoint(date, close);
                }
                return null;
            })
            .Where(p => p != null)
            .Cast<PricePoint>()
            .OrderBy(p => p.Date)
            .TakeLast(30) // Take last 30 days
            .ToList();
    }

    private class AlphaVantageResponse
    {
        [JsonPropertyName("Time Series (Daily)")]
        public Dictionary<string, AlphaVantageDailyData>? TimeSeries { get; set; }
    }

    private class AlphaVantageDailyData
    {
        [JsonPropertyName("4. close")]
        public string? Close { get; set; }
    }
}
