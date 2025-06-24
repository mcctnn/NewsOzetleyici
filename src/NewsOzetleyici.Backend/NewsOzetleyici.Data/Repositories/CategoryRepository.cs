using Microsoft.EntityFrameworkCore;
using NewsOzetleyici.Core.Entities;
using NewsOzetleyici.Data.Context;

namespace NewsOzetleyici.Data.Repositories;
public class CategoryRepository : Repository<Category>
{
    public CategoryRepository(NewsDbContext context) : base(context)
    {
    }

    public async Task<Category?> GetByNameAsync(string name)
    {
        return await _dbSet.FirstOrDefaultAsync(c => c.Name.ToLower() == name.ToLower());
    }

    public async Task<IEnumerable<Category>> GetCategoriesWithNewsCountAsync()
    {
        return await _dbSet
            .Include(c => c.News)
            .ToListAsync();
    }
}
