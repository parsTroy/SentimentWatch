using StockSentiment.Core.Models;

namespace StockSentiment.Web.Services;

public interface ISentimentClient
{
    Task<SentimentSnapshot?> GetSentimentAsync(string symbol, CancellationToken cancellationToken = default);
}


