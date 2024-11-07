using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Diagnostics;

using PPWCode.Vernacular.Exceptions.IV;
using PPWCode.Vernacular.Persistence.V;
using PPWCode.Vernacular.RequestContext.I;

namespace PPWCode.Vernacular.EntityFrameworkCore.I;

public abstract class AuditableInterceptor<TTimestamp> : SaveChangesInterceptor
    where TTimestamp : struct, IComparable<TTimestamp>, IEquatable<TTimestamp>
{
    private readonly IRequestContext<TTimestamp> _requestContext;

    protected AuditableInterceptor(IRequestContext<TTimestamp> requestContext)
    {
        _requestContext = requestContext;
    }

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        if (eventData.Context is not null)
        {
            UpdateAuditableEntities(eventData.Context);
        }
        else
        {
            throw new ProgrammingError("Expected context to be initialized.");
        }

        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    protected virtual void UpdateAuditableEntities(DbContext context)
    {
        object requestTimestamp = _requestContext.RequestTimestamp;
        object identityName = _requestContext.IdentityName;

        foreach (EntityEntry<IInsertAuditable<TTimestamp>> entityEntry in
                 context
                     .ChangeTracker
                     .Entries<IInsertAuditable<TTimestamp>>()
                     .Where(e => e.State == EntityState.Added))
        {
            entityEntry.Property(nameof(IInsertAuditable<TTimestamp>.CreatedAt)).CurrentValue = requestTimestamp;
            entityEntry.Property(nameof(IInsertAuditable<TTimestamp>.CreatedBy)).CurrentValue = identityName;
        }

        foreach (EntityEntry<IUpdateAuditable<TTimestamp>> entityEntry in
                 context
                     .ChangeTracker
                     .Entries<IUpdateAuditable<TTimestamp>>()
                     .Where(e => e.State == EntityState.Modified))
        {
            entityEntry.Property(nameof(IUpdateAuditable<TTimestamp>.LastModifiedAt)).CurrentValue = requestTimestamp;
            entityEntry.Property(nameof(IUpdateAuditable<TTimestamp>.LastModifiedBy)).CurrentValue = identityName;
        }
    }
}
