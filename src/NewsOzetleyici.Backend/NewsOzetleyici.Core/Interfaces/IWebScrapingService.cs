namespace NewsOzetleyici.Core.Interfaces;
public interface IWebScrapingService
{
    Task<ScrapedNews> ScrapeNewsAsync(string url);
    Task<bool> IsValidNewsUrlAsync(string url);
}

public class ScrapedNews
{
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public string? Author { get; set; }
    public DateTime? PublishedAt { get; set; }
    public string? ImageUrl { get; set; }
}
