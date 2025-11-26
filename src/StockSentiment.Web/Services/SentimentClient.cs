using System.Net.Http.Json;
using StockSentiment.Core.Models;

namespace StockSentiment.Web.Services;

public sealed class SentimentClient : ISentimentClient
{
    private readonly HttpClient httpClient;

    public SentimentClient(HttpClient httpClient)
    {
        this.httpClient = httpClient;
    }

    public async Task<SentimentSnapshot?> GetSentimentAsync(string symbol, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(symbol))
        {
            return null;
        }

        var encodedSymbol = Uri.EscapeDataString(symbol.Trim());
        
        try
        {
            var response = await httpClient.GetAsync($"/api/sentiment/{encodedSymbol}", cancellationToken);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<SentimentSnapshot>(cancellationToken);
        }
        catch (HttpRequestException ex)
        {
            throw new HttpRequestException(
                $"Failed to connect to sentiment API at {httpClient.BaseAddress}. " +
                $"Make sure the API is running. Original error: {ex.Message}", 
                ex);
        }
    }
}


