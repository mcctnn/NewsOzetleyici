using Microsoft.EntityFrameworkCore;
using NewsOzetleyici.Core.Interfaces;
using NewsOzetleyici.Data.Context;
using System.Linq.Expressions;

namespace NewsOzetleyici.Data.Repositories;
public class Repository<T> : IRepository<T> where T : class
{
    protected readonly NewsDbContext _context;
    protected readonly DbSet<T> _dbSet;

    public Repository(NewsDbContext context)
    {
        _context = context;
        _dbSet = context.Set<T>();
    }

    public virtual async Task<T> AddAsync(T entity)
    {
        await _dbSet.AddAsync(entity);
        await _context.SaveChangesAsync();

        return entity;
    }

    public virtual async Task<Int32> CountAsync()
    {
        return await _dbSet.CountAsync();
    }

    public virtual async Task DeleteAsync(Int32 id)
    {
        var entity = await GetByIdAsync(id);
        if (entity !=null)
        {
            _dbSet.Remove(entity);
            await _context.SaveChangesAsync();
        }
    }

    public virtual async Task<Boolean> ExistsAsync(Int32 id)
    {
        return await _dbSet.FindAsync(id) != null;
    }

    public virtual async Task<IEnumerable<T>> FindAsync(Expression<Func<T, Boolean>> predicate)
    {
        return await _dbSet.Where(predicate).ToListAsync();
    }

    public virtual async Task<T?> FirstOrDefaultAsync(Expression<Func<T, Boolean>> predicate)
    {
        return await _dbSet.FirstOrDefaultAsync(predicate);
    }

    public virtual async Task<IEnumerable<T>> GetAllAsync()
    {
        return await _dbSet.ToListAsync();
    }

    public virtual async Task<T?> GetByIdAsync(Int32 id)
    {
        return await _dbSet.FindAsync(id);
    }

    public virtual async Task<T> UpdateAsync(T entity)
    {
        _dbSet.Update(entity);
        await _context.SaveChangesAsync();
        return entity;
    }
}
