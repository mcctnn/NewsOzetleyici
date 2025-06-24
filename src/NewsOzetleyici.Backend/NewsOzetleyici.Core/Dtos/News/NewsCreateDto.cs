using System.ComponentModel.DataAnnotations;

namespace NewsOzetleyici.Core.Dtos.News;
public sealed record NewsCreateDto
{
    [Required]
    [Url]
    public string Url { get; init; } = string.Empty;

    [Range(1, 5)]
    public int SummaryLength { get; init; } = 3;

    [MaxLength(10)]
    public string Language { get; init; } = "tr";
}
