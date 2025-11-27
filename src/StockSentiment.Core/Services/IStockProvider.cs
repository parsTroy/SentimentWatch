using StockSentiment.Core.Models;

namespace StockSentiment.Core.Services;

public interface IStockProvider
{
    Task<IReadOnlyList<PricePoint>> GetPriceHistoryAsync(string symbol, CancellationToken cancellationToken = default);
}
