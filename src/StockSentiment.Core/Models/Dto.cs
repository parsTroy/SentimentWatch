namespace StockSentiment.Core.Models
{
    public record PricePoint(DateTimeOffset Date, decimal Close);

    public record NewsItem(
        string Headline,
        string Url,
        DateTimeOffset PublishedAt,
        string Source
    );

    public record NewsWithSentiment(
        string Headline,
        string Url,
        DateTimeOffset PublishedAt,
        string Source,
        string Sentiment,
        float Score
    );

    public record SentimentSnapshot(
        string Symbol,
        IReadOnlyList<PricePoint> PriceHistory,
        IReadOnlyList<NewsWithSentiment> News,
        float AverageScore,
        string OverallSentiment
    );
}
