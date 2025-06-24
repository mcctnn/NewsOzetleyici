namespace NewsOzetleyici.Core.Interfaces;
public interface IAiSummarizationService
{
    Task<SummarizationResponse> SummarizeAsync(SummarizationRequest request);
    Task<string> DetectCategoryAsync(string title, string content);
}

public sealed record SummarizationRequest
{
    public string Text { get; init; } = string.Empty;
    public int SummaryLength { get; init; } = 3; // 1-5 arası
    public string Language { get; init; } = "tr";
}

public sealed record SummarizationResponse
{
    public string SummaryText { get; init; } = string.Empty;
    public float ConfidenceScore { get; init; }
    public int ProcessingTimeMs { get; init; }
    public string ModelUsed { get; init; } = string.Empty;
    public bool IsSuccess { get; init; }
    public string? ErrorMessage { get; init; }
}
