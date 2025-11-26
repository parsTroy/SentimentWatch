using StockSentiment.Core.Models;

namespace StockSentiment.Core.Services;

public interface ISentimentService
{
    Task<SentimentSnapshot> GetSentimentAsync(string symbol, CancellationToken cancellationToken = default);
}


