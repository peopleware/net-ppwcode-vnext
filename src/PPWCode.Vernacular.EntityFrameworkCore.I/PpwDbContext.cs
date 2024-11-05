using System.Reflection;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;

using PPWCode.Vernacular.EntityFrameworkCore.I.ModelFinalizingConventions;
using PPWCode.Vernacular.Persistence.V;

namespace PPWCode.Vernacular.EntityFrameworkCore.I;

public abstract class PpwDbContext<TTimestamp> : DbContext
    where TTimestamp : struct, IComparable<TTimestamp>, IEquatable<TTimestamp>
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

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        foreach (Assembly configurationAssembly in ConfigurationAssemblies)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(configurationAssembly);
        }

        // Be sure that the types are correct for audit record stamping
        foreach (IMutableEntityType entity in modelBuilder.Model.GetEntityTypes())
        {
            List<Type> interfaces =
                entity
                    .ClrType.GetInterfaces()
                    .Where(i => i.IsGenericType)
                    .ToList();

            if (interfaces.Any(i => i.GetGenericTypeDefinition() == typeof(IInsertAuditable<>)))
            {
                IMutableProperty? createdByProperty = entity.FindProperty(nameof(IInsertAuditable<TTimestamp>.CreatedBy));
                createdByProperty?.SetMaxLength(AuditColumnLength);
            }

            if (interfaces.Any(i => i.GetGenericTypeDefinition() == typeof(IUpdateAuditable<>)))
            {
                IMutableProperty? createdByProperty = entity.FindProperty(nameof(IUpdateAuditable<TTimestamp>.LastModifiedBy));
                createdByProperty?.SetMaxLength(AuditColumnLength);
            }
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
