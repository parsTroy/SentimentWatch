using System.Text.Json.Serialization;
using Microsoft.Extensions.Configuration;
using StockSentiment.Core.Models;
using StockSentiment.Core.Services;
using System.Net.Http.Json;

namespace StockSentiment.Infrastructure.Services;

public class NewsApiProvider : INewsProvider
{
    private readonly HttpClient _httpClient;
    private readonly string _apiKey;

    public NewsApiProvider(HttpClient httpClient, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _apiKey = configuration["NewsData:ApiKey"] ?? throw new InvalidOperationException("NewsData:ApiKey is missing");
        
        // NewsAPI requires a User-Agent
        _httpClient.DefaultRequestHeaders.Add("User-Agent", "StockSentimentApp");
    }

    public async Task<IReadOnlyList<NewsItem>> GetNewsAsync(string symbol, CancellationToken cancellationToken = default)
    {
        var url = $"https://newsapi.org/v2/everything?q={symbol}&language=en&sortBy=publishedAt&pageSize=10&apiKey={_apiKey}";
        
        try 
        {
            var response = await _httpClient.GetFromJsonAsync<NewsApiResponse>(url, cancellationToken);

            if (response?.Articles == null)
            {
                return Array.Empty<NewsItem>();
            }

            return response.Articles
                .Select(a => new NewsItem(
                    a.Title ?? "No Title",
                    a.Url ?? "#",
                    a.PublishedAt,
                    a.Source?.Name ?? "Unknown"
                ))
                .ToList();
        }
        catch (Exception)
        {
            // Fallback or log error
            return Array.Empty<NewsItem>();
        }
    }

    private class NewsApiResponse
    {
        [JsonPropertyName("articles")]
        public List<Article>? Articles { get; set; }
    }

    private class Article
    {
        [JsonPropertyName("title")]
        public string? Title { get; set; }

        [JsonPropertyName("url")]
        public string? Url { get; set; }

        [JsonPropertyName("publishedAt")]
        public DateTimeOffset PublishedAt { get; set; }

        [JsonPropertyName("source")]
        public Source? Source { get; set; }
    }

    private class Source
    {
        [JsonPropertyName("name")]
        public string? Name { get; set; }
    }
}
