using NewsOzetleyici.Core.Dtos.Category;
using NewsOzetleyici.Core.Dtos.News;
using NewsOzetleyici.Core.Dtos.Summary;
using NewsOzetleyici.Core.Entities;
using NewsOzetleyici.Core.Interfaces;
using NewsOzetleyici.Data.Repositories;

namespace NewsOzetleyici.Services.Services;
public class NewsService
{
    private readonly INewsRepository _newsRepository;
    private readonly IWebScrapingService _webScrapingService;
    private readonly IAiSummarizationService _aiService;
    private readonly CategoryRepository _categoryRepository;

    public NewsService(
        INewsRepository newsRepository,
        IWebScrapingService webScrapingService,
        IAiSummarizationService aiService,
        CategoryRepository categoryRepository)
    {
        _newsRepository = newsRepository;
        _webScrapingService = webScrapingService;
        _aiService = aiService;
        _categoryRepository = categoryRepository;
    }

    public async Task<NewsResponseDto> ProcessNewsAsync(NewsCreateDto request)
    {
        // 1. URL'nin daha önce işlenip işlenmediğini kontrol et
        var existingNews = await _newsRepository.GetNewsByUrlAsync(request.Url);
        if (existingNews != null)
        {
            return MapToResponseDto(existingNews);
        }

        // 2. URL'den haber verilerini çek
        var scrapedNews = await _webScrapingService.ScrapeNewsAsync(request.Url);

        // 3. Kategori tespit et
        var categoryName = await _aiService.DetectCategoryAsync(scrapedNews.Title, scrapedNews.Content);
        var category = await _categoryRepository.GetByNameAsync(categoryName)
            ?? await _categoryRepository.GetByNameAsync("Genel");

        // 4. News entity'sini oluştur
        var news = new News
        {
            Url = request.Url,
            Title = scrapedNews.Title,
            Content = scrapedNews.Content,
            Author = scrapedNews.Author,
            PublishedAt = scrapedNews.PublishedAt,
            ImageUrl = scrapedNews.ImageUrl,
            CategoryId = category!.Id
        };

        // 5. Veritabanına kaydet
        var savedNews = await _newsRepository.AddAsync(news);

        // 6. Özet oluştur
        var summaryRequest = new SummarizationRequest
        {
            Text = scrapedNews.Content,
            SummaryLength = request.SummaryLength,
            Language = request.Language
        };

        var summaryResponse = await _aiService.SummarizeAsync(summaryRequest);

        // 7. Özeti kaydet
        var summary = new Summary
        {
            NewsId = savedNews.Id,
            SummaryText = summaryResponse.SummaryText,
            SummaryLength = request.SummaryLength,
            Language = request.Language,
            AiModel = summaryResponse.ModelUsed,
            ConfidenceScore = summaryResponse.ConfidenceScore,
            ProcessingTimeMs = summaryResponse.ProcessingTimeMs
        };

        // Summary'yi news'e ekle (manuel olarak çünkü henüz repository yok)
        savedNews.Summaries.Add(summary);
        savedNews.Category = category;

        return MapToResponseDto(savedNews);
    }

    public async Task<IEnumerable<NewsResponseDto>> GetAllNewsAsync()
    {
        var newsList = await _newsRepository.GetAllAsync();
        return newsList.Select(MapToResponseDto);
    }

    public async Task<NewsResponseDto?> GetNewsByIdAsync(int id)
    {
        var news = await _newsRepository.GetByIdAsync(id);
        return news != null ? MapToResponseDto(news) : null;
    }

    public async Task<IEnumerable<NewsResponseDto>> GetNewsByCategoryAsync(int categoryId)
    {
        var newsList = await _newsRepository.GetNewsByCategoryAsync(categoryId);
        return newsList.Select(MapToResponseDto);
    }

    public async Task<NewsResponseDto> ToggleFavoriteAsync(int id)
    {
        var news = await _newsRepository.GetByIdAsync(id);
        if (news == null) throw new ArgumentException("Haber bulunamadı");

        news.IsFavorite = !news.IsFavorite;
        news.UpdatedAt = DateTime.UtcNow;

        var updatedNews = await _newsRepository.UpdateAsync(news);
        return MapToResponseDto(updatedNews);
    }

    private NewsResponseDto MapToResponseDto(News news)
    {
        return new NewsResponseDto
        {
            Id = news.Id,
            Url = news.Url,
            Title = news.Title,
            Content = news.Content,
            Author = news.Author,
            PublishedAt = news.PublishedAt,
            ImageUrl = news.ImageUrl,
            IsFavorite = news.IsFavorite,
            CreatedAt = news.CreatedAt,
            Category = new CategoryDto
            {
                Id = news.Category.Id,
                Name = news.Category.Name,
                Description = news.Category.Description,
                Color = news.Category.Color
            },
            Summaries = news.Summaries?.Select(s => new SummaryDto
            {
                Id = s.Id,
                SummaryText = s.SummaryText,
                SummaryLength = s.SummaryLength,
                Language = s.Language,
                AiModel = s.AiModel,
                ConfidenceScore = s.ConfidenceScore,
                ProcessingTimeMs = s.ProcessingTimeMs,
                CreatedAt = s.CreatedAt
            }).ToList() ?? new List<SummaryDto>()
        };
    }
}
