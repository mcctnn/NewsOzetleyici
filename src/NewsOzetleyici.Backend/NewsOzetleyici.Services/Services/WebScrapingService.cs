using HtmlAgilityPack;
using NewsOzetleyici.Core.Interfaces;
using System.Text.RegularExpressions;

namespace NewsOzetleyici.Services.Services;
public class WebScrapingService : IWebScrapingService
{
    private readonly HttpClient _httpClient;

    public WebScrapingService(HttpClient httpClient)
    {
        _httpClient = httpClient;
        _httpClient.DefaultRequestHeaders.Add("User-Agent",
            "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36");
    }

    public async Task<ScrapedNews> ScrapeNewsAsync(string url)
    {
        try
        {
            var response = await _httpClient.GetStringAsync(url);
            var doc = new HtmlDocument();
            doc.LoadHtml(response);

            var scrapedNews = new ScrapedNews();

            // Başlık çekme - çoklu selector dene
            scrapedNews.Title = ExtractTitle(doc);

            // İçerik çekme
            scrapedNews.Content = ExtractContent(doc);

            // Yazar çekme
            scrapedNews.Author = ExtractAuthor(doc);

            // Resim URL çekme
            scrapedNews.ImageUrl = ExtractImageUrl(doc, url);

            // Yayın tarihi çekme
            scrapedNews.PublishedAt = ExtractPublishDate(doc);

            return scrapedNews;
        }
        catch (Exception ex)
        {
            throw new Exception($"Web scraping hatası: {ex.Message}");
        }
    }

    public async Task<bool> IsValidNewsUrlAsync(string url)
    {
        try
        {
            if (!Uri.IsWellFormedUriString(url, UriKind.Absolute)) return false;

            var response = await _httpClient.SendAsync(new HttpRequestMessage(HttpMethod.Head, url));
            return response.IsSuccessStatusCode &&
                   response.Content.Headers.ContentType?.MediaType?.Contains("text/html") == true;
        }
        catch
        {
            return false;
        }
    }

    private string ExtractTitle(HtmlDocument doc)
    {
        // XPath selectors - düzeltilmiş
        var xpathSelectors = new[]
        {
            "//h1[@class='entry-title']",
            "//h1[@class='post-title']",
            "//h1[@class='article-title']",
            "//h1[contains(@class, 'title')]",
            "//div[@class='entry-header']//h1",
            "//div[@class='post-header']//h1",
            "//h1",
            "//title"
        };

        foreach (var xpath in xpathSelectors)
        {
            try
            {
                var titleNode = doc.DocumentNode.SelectSingleNode(xpath);
                if (titleNode != null && !string.IsNullOrWhiteSpace(titleNode.InnerText))
                {
                    return CleanText(titleNode.InnerText);
                }
            }
            catch (Exception)
            {
                // XPath hatası durumunda bir sonrakine geç
                continue;
            }
        }

        return "Başlık Bulunamadı";
    }

    private string ExtractContent(HtmlDocument doc)
    {
        // XPath selectors - düzeltilmiş
        var xpathSelectors = new[]
        {
            "//div[@class='entry-content']",
            "//div[@class='post-content']",
            "//div[@class='article-content']",
            "//div[@class='content']",
            "//div[@class='post-body']",
            "//div[@class='article-body']",
            "//div[contains(@class, 'content')]",
            "//div[@class='story-body']"
        };

        foreach (var xpath in xpathSelectors)
        {
            try
            {
                var contentNode = doc.DocumentNode.SelectSingleNode(xpath);
                if (contentNode != null)
                {
                    // Script ve style taglerini temizle
                    var scripts = contentNode.SelectNodes(".//script | .//style");
                    if (scripts != null)
                    {
                        foreach (var script in scripts)
                            script.Remove();
                    }

                    var content = contentNode.InnerText;
                    if (!string.IsNullOrWhiteSpace(content) && content.Length > 100)
                    {
                        return CleanText(content);
                    }
                }
            }
            catch (Exception)
            {
                // XPath hatası durumunda bir sonrakine geç
                continue;
            }
        }

        // Fallback: Tüm p taglerini al
        try
        {
            var paragraphs = doc.DocumentNode.SelectNodes("//p");
            if (paragraphs != null)
            {
                var allText = string.Join(" ", paragraphs.Select(p => CleanText(p.InnerText)));
                if (allText.Length > 100) return allText;
            }
        }
        catch (Exception)
        {
            // XPath hatası durumunda boş döndür
        }

        return "İçerik çekilemedi";
    }

    private string? ExtractAuthor(HtmlDocument doc)
    {
        var xpathSelectors = new[]
        {
            "//div[@class='author']",
            "//div[@class='byline']",
            "//div[@class='post-author']",
            "//div[contains(@class, 'author')]",
            "//*[@rel='author']",
            "//span[@class='author']",
            "//a[@rel='author']"
        };

        foreach (var xpath in xpathSelectors)
        {
            try
            {
                var authorNode = doc.DocumentNode.SelectSingleNode(xpath);
                if (authorNode != null && !string.IsNullOrWhiteSpace(authorNode.InnerText))
                {
                    return CleanText(authorNode.InnerText);
                }
            }
            catch (Exception)
            {
                // XPath hatası durumunda bir sonrakine geç
                continue;
            }
        }

        return null;
    }

    private string? ExtractImageUrl(HtmlDocument doc, string baseUrl)
    {
        var xpathSelectors = new[]
        {
            "//div[@class='featured-image']//img",
            "//div[@class='post-image']//img",
            "//div[@class='article-image']//img",
            "//div[@class='entry-content']//img",
            "//img[contains(@class, 'featured')]",
            "//img[1]" // İlk resmi al
        };

        foreach (var xpath in xpathSelectors)
        {
            try
            {
                var imgNode = doc.DocumentNode.SelectSingleNode(xpath);
                if (imgNode != null)
                {
                    var src = imgNode.GetAttributeValue("src", "");
                    if (!string.IsNullOrEmpty(src))
                    {
                        return Uri.IsWellFormedUriString(src, UriKind.Absolute)
                            ? src
                            : new Uri(new Uri(baseUrl), src).ToString();
                    }
                }
            }
            catch (Exception)
            {
                // XPath hatası durumunda bir sonrakine geç
                continue;
            }
        }

        return null;
    }

    private DateTime? ExtractPublishDate(HtmlDocument doc)
    {
        var xpathSelectors = new[]
        {
            "//time[@datetime]",
            "//div[@class='published']",
            "//div[@class='post-date']",
            "//div[contains(@class, 'date')]",
            "//meta[@property='article:published_time']",
            "//span[@class='date']",
            "//time"
        };

        foreach (var xpath in xpathSelectors)
        {
            try
            {
                var dateNode = doc.DocumentNode.SelectSingleNode(xpath);
                if (dateNode != null)
                {
                    var dateText = dateNode.GetAttributeValue("datetime", "") ??
                                  dateNode.GetAttributeValue("content", "") ??
                                  dateNode.InnerText;

                    if (!string.IsNullOrEmpty(dateText) && DateTime.TryParse(dateText, out var date))
                    {
                        return date;
                    }
                }
            }
            catch (Exception)
            {
                // XPath hatası durumunda bir sonrakine geç
                continue;
            }
        }

        return null;
    }

    private string CleanText(string text)
    {
        if (string.IsNullOrEmpty(text)) return string.Empty;

        // HTML decode
        text = System.Net.WebUtility.HtmlDecode(text);

        // Çok fazla boşluk ve yeni satırları temizle
        text = Regex.Replace(text, @"\s+", " ");

        return text.Trim();
    }
}