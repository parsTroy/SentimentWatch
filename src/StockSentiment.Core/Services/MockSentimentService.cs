using StockSentiment.Core.Models;

namespace StockSentiment.Core.Services;

public sealed class MockSentimentService : ISentimentService
{
    private static readonly string[] SampleSources =
    {
        "MarketWatch", "Bloomberg", "Reuters", "CNBC"
    };

    private static readonly string[] SampleHeadlines =
    {
        "Stock extends rally after upbeat earnings report",
        "Analysts turn cautious amid macro uncertainty",
        "Investors eye upcoming product launch",
        "Regulatory concerns weigh on short-term outlook"
    };

    private readonly Random random = new();

    public Task<SentimentSnapshot> GetSentimentAsync(string symbol, CancellationToken cancellationToken = default)
    {
        symbol = symbol.Trim().ToUpperInvariant();

        var today = DateTimeOffset.UtcNow.Date;
        var priceHistory = Enumerable
            .Range(0, 10)
            .Select(offset =>
            {
                var date = today.AddDays(-offset);
                var close = 100m + (decimal)(random.NextDouble() * 10 - 5);
                return new PricePoint(date, decimal.Round(close, 2));
            })
            .OrderBy(p => p.Date)
            .ToList();

        var news = Enumerable
            .Range(0, 5)
            .Select(index =>
            {
                var headline = SampleHeadlines[index % SampleHeadlines.Length];
                var publishedAt = today.AddHours(-random.Next(1, 72));
                var source = SampleSources[index % SampleSources.Length];

                var score = (float)random.NextDouble();
                var sentiment = score switch
                {
                    > 0.66f => "Positive",
                    < 0.33f => "Negative",
                    _ => "Neutral"
                };

                return new NewsWithSentiment(
                    $"{symbol}: {headline}",
                    $"https://example.com/news/{symbol}/{index}",
                    publishedAt,
                    source,
                    sentiment,
                    score
                );
            })
            .OrderByDescending(n => n.PublishedAt)
            .ToList();

        var averageScore = news.Count == 0 ? 0f : news.Average(n => n.Score);
        var overallSentiment = averageScore switch
        {
            > 0.66f => "Positive",
            < 0.33f => "Negative",
            _ => "Neutral"
        };

        var snapshot = new SentimentSnapshot(
            symbol,
            priceHistory,
            news,
            averageScore,
            overallSentiment
        );

        return Task.FromResult(snapshot);
    }
}


