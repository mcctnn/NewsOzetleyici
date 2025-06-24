using NewsOzetleyici.Core.Entities;

namespace NewsOzetleyici.Core.Interfaces;
public interface INewsRepository:IRepository<News>
{
    Task<IEnumerable<News>> GetNewsByCategoryAsync(int categoryId);
    Task<IEnumerable<News>> GetFavoriteNewsAsync();
    Task<News?> GetNewsByUrlAsync(string url);
    Task<IEnumerable<News>> GetRecentNewsAsync(int count = 10);
}
