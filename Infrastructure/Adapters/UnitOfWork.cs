using Domain.Ports;
using Infrastructure.DataSource;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Adapters;

public class UnitOfWork : IUnitOfWork
{

    private readonly DataContext _context;
    public UnitOfWork(DataContext context)
    {
        _context = context;
    }
    public async Task SaveAsync(CancellationToken? cancellationToken = null)
    {
        var token = cancellationToken ?? new CancellationTokenSource().Token;

        _context.ChangeTracker.DetectChanges();
        
        var entryStatus = new Dictionary<EntityState, string> {
            {EntityState.Added, "CreatedOn"},
            {EntityState.Modified, "LastModifiedOn"}
        };

        foreach (var entry in _context.ChangeTracker.Entries())
        {
            if (entryStatus.ContainsKey(entry.State)) {
                entry.Property(entryStatus[entry.State]).CurrentValue = DateTime.UtcNow;
            }           
        }

        await _context.SaveChangesAsync(token);
    }
}
