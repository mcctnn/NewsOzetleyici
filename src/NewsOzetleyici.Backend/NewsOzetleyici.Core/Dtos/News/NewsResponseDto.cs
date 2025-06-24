using NewsOzetleyici.Core.Dtos.Category;
using NewsOzetleyici.Core.Dtos.Summary;

namespace NewsOzetleyici.Core.Dtos.News;
public sealed record NewsResponseDto
{
    public int Id { get; init; }
    public string Url { get; init; } = string.Empty;
    public string Title { get; init; } = string.Empty;
    public string Content { get; init; } = string.Empty;
    public string? Author { get; init; }
    public DateTime? PublishedAt { get; init; }
    public string? ImageUrl { get; init; }
    public bool IsFavorite { get; init; }
    public DateTime CreatedAt { get; init; }

    public CategoryDto Category { get; init; } = null!;
    public List<SummaryDto> Summaries { get; init; } = new();
}
