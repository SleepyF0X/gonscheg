using System.Linq.Expressions;
using Gonscheg.Domain;

namespace Gonscheg.Application.Repositories;

public interface IBaseCRUDRepository<T> where T : Entity
{
    public Task<T?> GetAsync(int id);
    public Task<T?> GetByAsync(Expression<Func<T, bool>> expression);
    public Task<T> CreateAsync(T entity);
    public Task<ICollection<T>> GetAllAsync();
    public Task<bool> UpdateAsync(T entity);
    public Task<bool> DeleteAsync(int id);
}