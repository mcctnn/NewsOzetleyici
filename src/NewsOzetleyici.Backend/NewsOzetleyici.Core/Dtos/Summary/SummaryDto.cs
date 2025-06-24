namespace NewsOzetleyici.Core.Dtos.Summary;
public sealed record SummaryDto
{
    public int Id { get; init; }
    public string SummaryText { get; init; } = string.Empty;
    public int SummaryLength { get; init; }
    public string Language { get; init; } = string.Empty;
    public string? AiModel { get; init; }
    public float? ConfidenceScore { get; init; }
    public int ProcessingTimeMs { get; init; }
    public DateTime CreatedAt { get; init; }
}
