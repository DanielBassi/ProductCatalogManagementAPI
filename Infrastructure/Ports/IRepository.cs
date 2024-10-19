using System.Linq.Expressions;
using Domain.Entities;

namespace Infrastructure.Ports;

public interface IRepository<T> where T : DomainEntity
{
    Task<T> AddAsync(T entity);
    Task DeleteAsync(T entity);

    Task<IEnumerable<T>> GetManyAsync(
        Expression<Func<T, bool>>? filter = null,
        Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
        string includeStringProperties = "",
        bool isTracking = false);

    Task<T?> GetOneAsync(Guid id);
    Task UpdateAsync(T entity);
}
