using System.ComponentModel.DataAnnotations;

namespace NewsOzetleyici.Core.Entities;
public class News : BaseEntity
{
    [Required]
    [MaxLength(2000)]
    public string Url { get; set; } = string.Empty;

    [Required]
    [MaxLength(500)]
    public string Title { get; set; } = string.Empty;

    [Required]
    public string Content { get; set; } = string.Empty;

    [MaxLength(100)]
    public string? Author { get; set; }

    public DateTime? PublishedAt { get; set; }

    [MaxLength(200)]
    public string? ImageUrl { get; set; }

    [Required]
    public int CategoryId { get; set; }

    public bool IsFavorite { get; set; } = false;

    public virtual Category Category { get; set; } = null!;
    public virtual ICollection<Summary> Summaries { get; set; } = new List<Summary>();
}
