using System.ComponentModel.DataAnnotations;

namespace NewsOzetleyici.Core.Entities;
public class Category:BaseEntity
{
    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = String.Empty;

    [MaxLength(500)]
    public string? Description { get; set; }

    [MaxLength(50)]
    public string Color { get; set; } = "#3f51b5";

    public virtual ICollection<News> News { get; set; } = new List<News>();
}
