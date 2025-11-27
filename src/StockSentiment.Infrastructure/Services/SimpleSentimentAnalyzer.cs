using StockSentiment.Core.Services;

namespace StockSentiment.Infrastructure.Services;

/// <summary>
/// Enhanced keyword-based sentiment analyzer with financial domain vocabulary.
/// This is a local, lightweight implementation that can be replaced with Azure AI or ML.NET in the future.
/// </summary>
public class SimpleSentimentAnalyzer : ISentimentAnalyzer
{
    // Positive financial keywords with weights
    private static readonly Dictionary<string, float> PositiveKeywords = new()
    {
        // Strong positive
        { "surge", 1.0f }, { "soar", 1.0f }, { "rally", 1.0f }, { "breakthrough", 1.0f },
        { "record high", 1.0f }, { "outperform", 1.0f }, { "beat expectations", 1.0f },
        
        // Moderate positive
        { "gain", 0.7f }, { "rise", 0.7f }, { "up", 0.7f }, { "growth", 0.7f },
        { "profit", 0.7f }, { "bull", 0.7f }, { "strong", 0.7f }, { "positive", 0.7f },
        { "upgrade", 0.7f }, { "buy", 0.7f }, { "optimistic", 0.7f }, { "improve", 0.7f },
        { "success", 0.7f }, { "advance", 0.7f }, { "boost", 0.7f }, { "expand", 0.7f },
        
        // Mild positive
        { "stable", 0.4f }, { "steady", 0.4f }, { "maintain", 0.4f }, { "hold", 0.4f }
    };

    // Negative financial keywords with weights
    private static readonly Dictionary<string, float> NegativeKeywords = new()
    {
        // Strong negative
        { "plunge", 1.0f }, { "crash", 1.0f }, { "collapse", 1.0f }, { "tumble", 1.0f },
        { "plummet", 1.0f }, { "disaster", 1.0f }, { "crisis", 1.0f }, { "bankruptcy", 1.0f },
        
        // Moderate negative
        { "fall", 0.7f }, { "drop", 0.7f }, { "down", 0.7f }, { "loss", 0.7f },
        { "bear", 0.7f }, { "decline", 0.7f }, { "weak", 0.7f }, { "negative", 0.7f },
        { "downgrade", 0.7f }, { "sell", 0.7f }, { "pessimistic", 0.7f }, { "concern", 0.7f },
        { "risk", 0.7f }, { "fail", 0.7f }, { "struggle", 0.7f }, { "warning", 0.7f },
        
        // Mild negative
        { "uncertain", 0.4f }, { "volatile", 0.4f }, { "cautious", 0.4f }, { "mixed", 0.4f }
    };

    // Negation words that flip sentiment
    private static readonly string[] NegationWords = { "not", "no", "never", "neither", "nor", "without", "lack" };

    public Task<SentimentResult> AnalyzeAsync(string text, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            return Task.FromResult(new SentimentResult("Neutral", 0.5f));
        }

        var lowerText = text.ToLowerInvariant();
        var words = lowerText.Split(new[] { ' ', ',', '.', '!', '?', ';', ':', '-' }, StringSplitOptions.RemoveEmptyEntries);

        float positiveScore = 0f;
        float negativeScore = 0f;

        // Analyze each word with context
        for (int i = 0; i < words.Length; i++)
        {
            var word = words[i];
            bool hasNegation = i > 0 && NegationWords.Contains(words[i - 1]);

            // Check for multi-word phrases first
            if (i < words.Length - 1)
            {
                var twoWordPhrase = $"{word} {words[i + 1]}";
                if (PositiveKeywords.TryGetValue(twoWordPhrase, out var posWeight))
                {
                    positiveScore += hasNegation ? -posWeight : posWeight;
                    continue;
                }
                if (NegativeKeywords.TryGetValue(twoWordPhrase, out var negWeight))
                {
                    negativeScore += hasNegation ? -negWeight : negWeight;
                    continue;
                }
            }

            // Check single words
            if (PositiveKeywords.TryGetValue(word, out var posWordWeight))
            {
                positiveScore += hasNegation ? -posWordWeight : posWordWeight;
            }
            else if (NegativeKeywords.TryGetValue(word, out var negWordWeight))
            {
                negativeScore += hasNegation ? -negWordWeight : negWordWeight;
            }
        }

        // Calculate final score (0.0 = very negative, 0.5 = neutral, 1.0 = very positive)
        var totalScore = positiveScore + negativeScore;
        float normalizedScore;
        string sentiment;

        if (Math.Abs(totalScore) < 0.1f)
        {
            // No significant sentiment detected
            normalizedScore = 0.5f;
            sentiment = "Neutral";
        }
        else
        {
            // Normalize to 0-1 range with sigmoid-like function
            var rawScore = positiveScore - negativeScore;
            normalizedScore = 0.5f + (rawScore / (Math.Abs(rawScore) + 2f)) * 0.5f;
            
            // Clamp to valid range
            normalizedScore = Math.Clamp(normalizedScore, 0f, 1f);

            // Determine sentiment label
            sentiment = normalizedScore switch
            {
                >= 0.7f => "Positive",
                <= 0.3f => "Negative",
                _ => "Neutral"
            };
        }

        return Task.FromResult(new SentimentResult(sentiment, normalizedScore));
    }
}
