using System.ComponentModel.DataAnnotations;

namespace NewsOzetleyici.Core.Entities;
public class Summary : BaseEntity
{
    [Required]
    public int NewsId { get; set; }

    [Required]
    public string SummaryText { get; set; } = string.Empty;

    [Range(1, 5)]
    public int SummaryLength { get; set; } = 3; // 1=Çok kısa, 5=Detaylı

    [MaxLength(50)]
    public string Language { get; set; } = "tr"; // tr, en

    [MaxLength(100)]
    public string? AiModel { get; set; } // Hangi AI modeli kullanıldı

    public float? ConfidenceScore { get; set; } // AI güven skoru

    public int ProcessingTimeMs { get; set; } // İşlem süresi

    public virtual News News { get; set; } = null!;
}
