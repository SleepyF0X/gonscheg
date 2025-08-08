using System.Linq.Expressions;
using Gonscheg.Application.Repositories;
using Gonscheg.Domain;
using Gonscheg.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Gonscheg.Infrastructure.Repositories;

public class BaseCRUDRepository<T> : IBaseCRUDRepository<T> where T : Entity
{
    public static DataContext _context;

    public BaseCRUDRepository(DataContext context)
    {
        _context = context;
    }

    public async Task<T?> GetAsync(int id)
    {
        return await _context
            .Set<T>()
            .AsNoTracking()
            .FirstOrDefaultAsync(e => e.Id == id);
    }

    public async Task<T?> GetByAsync(Expression<Func<T, bool>> expression)
    {
        return await _context
            .Set<T>()
            .AsNoTracking()
            .FirstOrDefaultAsync(expression);
    }

    public async Task<ICollection<T>> GetAllByAsync(Expression<Func<T, bool>> expression)
    {
        return await _context
            .Set<T>()
            .AsNoTracking()
            .Where(expression)
            .ToListAsync();
    }

    public async Task<T> CreateAsync(T entity)
    {
        var result = await _context
            .AddAsync(entity);
        await _context.SaveChangesAsync();
        return result.Entity;
    }

    public async Task<ICollection<T>> GetAllAsync()
    {
        return await _context
            .Set<T>()
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<bool> UpdateAsync(T entity)
    {
        var result = _context
            .Set<T>()
            .Update(entity);
        await _context.SaveChangesAsync();
        return result.State == EntityState.Modified;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var res = await _context.Set<T>()
            .Where(e => e.Id == id)
            .ExecuteDeleteAsync() == 1;
        await _context.SaveChangesAsync();
        return res;
    }
}
