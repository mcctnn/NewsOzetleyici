using Microsoft.Extensions.Configuration;
using NewsOzetleyici.Core.Interfaces;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Text;

namespace NewsOzetleyici.Services.Services;

public class HuggingFaceService : IAiSummarizationService
{
    private readonly HttpClient _httpClient;
    private readonly string _apiKey;
    private readonly string _baseUrl = "https://api-inference.huggingface.co/models";

    public HuggingFaceService(HttpClient httpClient, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _apiKey = configuration["HuggingFace:ApiKey"] ?? throw new ArgumentNullException("HuggingFace API Key bulunamadı");

        _httpClient.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _apiKey);
    }

    public async Task<SummarizationResponse> SummarizeAsync(SummarizationRequest request)
    {
        var stopwatch = Stopwatch.StartNew();

        try
        {
            // Model seçimi - dil bazlı
            var model = request.Language.ToLower() == "tr"
                ? "facebook/bart-large-cnn"  // Türkçe için en iyi
                : "microsoft/DialoGPT-medium"; // Çok dilli için

            var url = $"{_baseUrl}/{model}";

            // Metin uzunluğunu kontrol et
            var text = request.Text.Length > 1000
                ? request.Text.Substring(0, 1000) + "..."
                : request.Text;

            // Özet uzunluğu parametreleri
            var (maxLength, minLength) = GetSummaryLengthParams(request.SummaryLength);

            var payload = new
            {
                inputs = text,
                parameters = new
                {
                    max_length = maxLength,
                    min_length = minLength,
                    do_sample = false,
                    early_stopping = true
                }
            };

            var json = JsonConvert.SerializeObject(payload);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync(url, content);
            var responseContent = await response.Content.ReadAsStringAsync();

            stopwatch.Stop();

            if (response.IsSuccessStatusCode)
            {
                var result = JsonConvert.DeserializeObject<HuggingFaceResponse[]>(responseContent);

                return new SummarizationResponse
                {
                    SummaryText = result?[0]?.summary_text ?? "Özet oluşturulamadı",
                    ConfidenceScore = CalculateConfidenceScore(result?[0]?.summary_text ?? ""),
                    ProcessingTimeMs = (int)stopwatch.ElapsedMilliseconds,
                    ModelUsed = model,
                    IsSuccess = true
                };
            }
            else
            {
                // Hata durumunda fallback özet
                return CreateFallbackSummary(request.Text, (int)stopwatch.ElapsedMilliseconds);
            }
        }
        catch (Exception ex)
        {
            stopwatch.Stop();

            return new SummarizationResponse
            {
                IsSuccess = false,
                ErrorMessage = ex.Message,
                ProcessingTimeMs = (int)stopwatch.ElapsedMilliseconds,
                SummaryText = CreateFallbackSummary(request.Text, (int)stopwatch.ElapsedMilliseconds).SummaryText
            };
        }
    }

    public async Task<string> DetectCategoryAsync(string title, string content)
    {
        try
        {
            // Basit keyword tabanlı kategori tespiti
            var text = (title + " " + content).ToLower();

            var categories = new Dictionary<string, string[]>
                {
                    { "Teknoloji", new[] { "teknoloji", "yazılım", "bilgisayar", "internet", "mobil", "app", "uygulama", "ai", "yapay zeka" } },
                    { "Spor", new[] { "futbol", "basketbol", "spor", "maç", "takım", "oyuncu", "galatasaray", "fenerbahçe", "beşiktaş" } },
                    { "Ekonomi", new[] { "ekonomi", "para", "dolar", "euro", "borsa", "yatırım", "banka", "finans", "kripto" } },
                    { "Sağlık", new[] { "sağlık", "doktor", "hastane", "tedavi", "ilaç", "hasta", "covid", "aşı", "tıp" } },
                    { "Bilim", new[] { "bilim", "araştırma", "keşif", "uzay", "nasa", "deney", "laboratuvar", "bilimsel" } },
                    { "Eğitim", new[] { "eğitim", "okul", "üniversite", "öğrenci", "öğretmen", "ders", "sınav", "mezun" } }
                };

            foreach (var category in categories)
            {
                if (category.Value.Any(keyword => text.Contains(keyword)))
                {
                    return category.Key;
                }
            }

            return "Genel";
        }
        catch
        {
            return "Genel";
        }
    }

    private (int maxLength, int minLength) GetSummaryLengthParams(int summaryLength)
    {
        return summaryLength switch
        {
            1 => (50, 20),   // Çok kısa
            2 => (100, 50),  // Kısa
            3 => (150, 75),  // Orta
            4 => (200, 100), // Uzun
            5 => (300, 150), // Çok uzun
            _ => (150, 75)   // Default
        };
    }

    private float CalculateConfidenceScore(string summary)
    {
        // Basit güven skoru hesaplama
        if (string.IsNullOrEmpty(summary)) return 0.1f;
        if (summary.Length < 20) return 0.3f;
        if (summary.Contains("özet") || summary.Contains("özetle")) return 0.6f;
        return 0.8f; // Normal durumda yüksek güven
    }

    private SummarizationResponse CreateFallbackSummary(string text, int processingTime)
    {
        // AI çalışmazsa basit özet oluştur
        var sentences = text.Split('.', StringSplitOptions.RemoveEmptyEntries);
        var summary = sentences.Length > 2
            ? string.Join(". ", sentences.Take(2)) + "."
            : text.Length > 200 ? text.Substring(0, 200) + "..." : text;

        return new SummarizationResponse
        {
            SummaryText = summary,
            ConfidenceScore = 0.5f,
            ProcessingTimeMs = processingTime,
            ModelUsed = "Fallback",
            IsSuccess = true
        };
    }
}

public sealed record HuggingFaceResponse
{
    public string summary_text { get; init; } = string.Empty;
}