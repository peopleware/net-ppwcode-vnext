using System.Reflection;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;

using PPWCode.Vernacular.EntityFrameworkCore.I.ModelFinalizingConventions;
using PPWCode.Vernacular.Persistence.V;

namespace PPWCode.Vernacular.EntityFrameworkCore.I;

public abstract class PpwDbContext : DbContext
{
    protected PpwDbContext(DbContextOptions options)
        : base(options)
    {
    }

    private static IDictionary<PpwConventions, Func<IServiceProvider, IConvention>> Conventions { get; } =
        new Dictionary<PpwConventions, Func<IServiceProvider, IConvention>>
        {
            { PpwConventions.INDICES, _ => new PpwIndexConvention() }
        };

    protected abstract IEnumerable<Assembly> ConfigurationAssemblies { get; }

    protected virtual int AuditColumnLength
        => 128;

    protected abstract PpwConventions? ConventionRequests { get; }

    public DbSet<TModel> GetDbSet<TModel, TId>()
        where TModel : class, IPersistentObject<TId>, IIdentity<TId>
        where TId : IEquatable<TId>
        => Set<TModel>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Be sure that the types are correct for audit record stamping
        foreach (IMutableEntityType entity in modelBuilder.Model.GetEntityTypes())
        {
            if (entity.ClrType.GetInterface(nameof(IInsertAuditable)) is not null)
            {
                IMutableProperty? createdByProperty = entity.FindProperty(nameof(IInsertAuditable.CreatedBy));
                createdByProperty?.SetMaxLength(AuditColumnLength);
            }

            if (entity.ClrType.GetInterface(nameof(IUpdateAuditable)) is not null)
            {
                IMutableProperty? createdByProperty = entity.FindProperty(nameof(IUpdateAuditable.LastModifiedBy));
                createdByProperty?.SetMaxLength(AuditColumnLength);
            }
        }

        foreach (Assembly configurationAssembly in ConfigurationAssemblies)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(configurationAssembly);
        }
    }

    /// <inheritdoc />
    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        base.ConfigureConventions(configurationBuilder);

        if (ConventionRequests is not null)
        {
            foreach (KeyValuePair<PpwConventions, Func<IServiceProvider, IConvention>> pair in Conventions)
            {
                if ((pair.Key & ConventionRequests.Value) == pair.Key)
                {
                    configurationBuilder.Conventions.Add(pair.Value);
                }
            }
        }
    }
}
