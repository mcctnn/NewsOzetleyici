using Microsoft.EntityFrameworkCore;
using NewsOzetleyici.Core.Entities;
using NewsOzetleyici.Core.Interfaces;
using NewsOzetleyici.Data.Context;

namespace NewsOzetleyici.Data.Repositories;
public class NewsRepository : Repository<News>, INewsRepository
{
    public NewsRepository(NewsDbContext context) : base(context)
    {
    }

    public override async Task<News?> GetByIdAsync(int id)
    {
        return await _dbSet
            .Include(n => n.Category)
            .Include(n => n.Summaries)
            .FirstOrDefaultAsync(n => n.Id == id);
    }

    public override async Task<IEnumerable<News>> GetAllAsync()
    {
        return await _dbSet
            .Include(n => n.Category)
            .Include(n => n.Summaries)
            .OrderByDescending(n => n.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<News>> GetNewsByCategoryAsync(int categoryId)
    {
        return await _dbSet
            .Include(n => n.Category)
            .Include(n => n.Summaries)
            .Where(n => n.CategoryId == categoryId)
            .OrderByDescending(n => n.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<News>> GetFavoriteNewsAsync()
    {
        return await _dbSet
            .Include(n => n.Category)
            .Include(n => n.Summaries)
            .Where(n => n.IsFavorite)
            .OrderByDescending(n => n.CreatedAt)
            .ToListAsync();
    }

    public async Task<News?> GetNewsByUrlAsync(string url)
    {
        return await _dbSet
            .Include(n => n.Category)
            .Include(n => n.Summaries)
            .FirstOrDefaultAsync(n => n.Url == url);
    }

    public async Task<IEnumerable<News>> GetRecentNewsAsync(int count = 10)
    {
        return await _dbSet
            .Include(n => n.Category)
            .Include(n => n.Summaries)
            .OrderByDescending(n => n.CreatedAt)
            .Take(count)
            .ToListAsync();
    }
}
