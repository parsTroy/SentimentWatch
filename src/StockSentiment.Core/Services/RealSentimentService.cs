using StockSentiment.Core.Models;

namespace StockSentiment.Core.Services;

public class RealSentimentService : ISentimentService
{
    private readonly IStockProvider _stockProvider;
    private readonly INewsProvider _newsProvider;
    private readonly ISentimentAnalyzer _sentimentAnalyzer;

    public RealSentimentService(
        IStockProvider stockProvider,
        INewsProvider newsProvider,
        ISentimentAnalyzer sentimentAnalyzer)
    {
        _stockProvider = stockProvider;
        _newsProvider = newsProvider;
        _sentimentAnalyzer = sentimentAnalyzer;
    }

    public async Task<SentimentSnapshot> GetSentimentAsync(string symbol, CancellationToken cancellationToken = default)
    {
        // Fetch data in parallel
        var priceTask = _stockProvider.GetPriceHistoryAsync(symbol, cancellationToken);
        var newsTask = _newsProvider.GetNewsAsync(symbol, cancellationToken);

        await Task.WhenAll(priceTask, newsTask);

        var prices = await priceTask;
        var newsItems = await newsTask;

        // Analyze sentiment for each news item
        // We can do this in parallel as well
        var analyzedNewsTasks = newsItems.Select(async news =>
        {
            var sentiment = await _sentimentAnalyzer.AnalyzeAsync(news.Headline, cancellationToken);
            return new NewsWithSentiment(
                news.Headline,
                news.Url,
                news.PublishedAt,
                news.Source,
                sentiment.Label,
                sentiment.Score
            );
        });

        var analyzedNews = (await Task.WhenAll(analyzedNewsTasks))
            .OrderByDescending(n => n.PublishedAt)
            .ToList();

        // Calculate aggregate stats
        var averageScore = analyzedNews.Count == 0 ? 0f : analyzedNews.Average(n => n.Score);
        var overallSentiment = averageScore switch
        {
            > 0.66f => "Positive",
            < 0.33f => "Negative",
            _ => "Neutral"
        };

        return new SentimentSnapshot(
            symbol,
            prices,
            analyzedNews,
            averageScore,
            overallSentiment
        );
    }
}
