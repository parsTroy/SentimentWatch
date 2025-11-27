namespace StockSentiment.Core.Services;

public record SentimentResult(string Label, float Score);

public interface ISentimentAnalyzer
{
    Task<SentimentResult> AnalyzeAsync(string text, CancellationToken cancellationToken = default);
}
