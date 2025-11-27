using StockSentiment.Core.Models;

namespace StockSentiment.Core.Services;

public interface INewsProvider
{
    Task<IReadOnlyList<NewsItem>> GetNewsAsync(string symbol, CancellationToken cancellationToken = default);
}
