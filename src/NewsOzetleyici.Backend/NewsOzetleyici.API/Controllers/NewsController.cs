using Microsoft.AspNetCore.Mvc;
using NewsOzetleyici.Core.Dtos.News;
using NewsOzetleyici.Services.Services;

namespace NewsOzetleyici.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class NewsController : ControllerBase
{
    private readonly NewsService _newsService;

    public NewsController(NewsService newsService)
    {
        _newsService = newsService;
    }

    [HttpPost]
    public async Task<ActionResult<NewsResponseDto>> CreateNews([FromBody] NewsCreateDto request)
    {
        try
        {
            var result = await _newsService.ProcessNewsAsync(request);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<NewsResponseDto>>> GetAllNews()
    {
        try
        {
            var result = await _newsService.GetAllNewsAsync();
            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<NewsResponseDto>> GetNewsById(int id)
    {
        try
        {
            var result = await _newsService.GetNewsByIdAsync(id);
            if (result == null)
                return NotFound(new { error = "Haber bulunamadı" });

            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpGet("category/{categoryId}")]
    public async Task<ActionResult<IEnumerable<NewsResponseDto>>> GetNewsByCategory(int categoryId)
    {
        try
        {
            var result = await _newsService.GetNewsByCategoryAsync(categoryId);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpPut("{id}/favorite")]
    public async Task<ActionResult<NewsResponseDto>> ToggleFavorite(int id)
    {
        try
        {
            var result = await _newsService.ToggleFavoriteAsync(id);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }
}
