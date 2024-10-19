using System.Linq.Expressions;
using Domain.Entities;
using Infrastructure.DataSource;
using Infrastructure.Ports;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Adapters;

public class GenericRepository<T> : IRepository<T> where T : DomainEntity
{
    private readonly DbSet<T> _dataset;
    private readonly DataContext Context;

    public GenericRepository(DataContext context)
    {
        Context = context ?? throw new ArgumentNullException(nameof(context));
        _dataset = Context.Set<T>();
    }

    public async Task<T> AddAsync(T entity)
    {
        _ = entity ?? throw new ArgumentNullException(nameof(entity), "Entity can not be null");
        await _dataset.AddAsync(entity);
        return entity;
    }

    public async Task DeleteAsync(T entity)
    {
        _ = entity ?? throw new ArgumentNullException(nameof(entity), "Entity can not be null");
        await Task.FromResult(_dataset.Remove(entity));
    }

    public async Task<IEnumerable<T>> GetManyAsync(
        Expression<Func<T, bool>>? filter = null,
        Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
        string includeStringProperties = "", bool isTracking = false)
    {
        IQueryable<T> query = Context.Set<T>();

        if (filter != null) query = query.Where(filter);

        if (!string.IsNullOrEmpty(includeStringProperties))
            foreach (var includeProperty in includeStringProperties.Split
                         (new[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                query = query.Include(includeProperty);

        if (orderBy != null) return await orderBy(query).ToListAsync().ConfigureAwait(false);

        return !isTracking ? await query.AsNoTracking().ToListAsync() : await query.ToListAsync();
    }

    public async Task<T?> GetOneAsync(Guid id)
    {
        return await _dataset.FindAsync(id);
    }

    public async Task UpdateAsync(T entity)
    {
        await Task.FromResult(_dataset.Update(entity));
    }
}
