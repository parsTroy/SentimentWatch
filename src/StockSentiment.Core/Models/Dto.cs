using System;

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
        string Sentiment,  // Positive, Negative, Neutral
        float Score        // confidence / probability from model
    );
}

