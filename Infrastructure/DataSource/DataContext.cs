using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Infrastructure.DataSource;

public class DataContext(DbContextOptions<DataContext> options, IConfiguration config)
    : DbContext(options)
{
    private readonly IConfiguration _config = config;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        if (modelBuilder is null) return;

        // load all entity config from current assembly
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(DataContext).Assembly);

        // if using schema in db, uncomment the following line
        // modelBuilder.HasDefaultSchema(_config.GetValue<string>("SchemaName"));
        modelBuilder.Entity<Product>();


        // ghost properties for audit
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            var t = entityType.ClrType;
            if (typeof(DomainEntity).IsAssignableFrom(t))
            {
                modelBuilder.Entity(entityType.Name).Property<DateTime>("CreatedOn");
                modelBuilder.Entity(entityType.Name).Property<DateTime>("LastModifiedOn");
            }
        }

        base.OnModelCreating(modelBuilder);
    }
}
