using Microsoft.EntityFrameworkCore;

using PPWCode.Vernacular.Persistence.V;
using PPWCode.Vernacular.Persistence.V.Exceptions;

namespace PPWCode.Vernacular.EntityFrameworkCore.I;

public abstract class Repository<TModel, TId> : IRepository<TModel, TId>
    where TModel : class, IPersistentObject<TId>, IIdentity<TId>
    where TId : IEquatable<TId>
{
    private readonly PpwDbContext _context;

    protected Repository(PpwDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    /// <inheritdoc />
    public virtual async Task<TModel?> GetByIdAsync(TId id, CancellationToken cancellationToken = default)
        => await DbSet
               .FindAsync([id], cancellationToken)
               .ConfigureAwait(false);

    /// <inheritdoc />
    public virtual async Task<List<TModel>> FindAllAsync(CancellationToken cancellationToken = default)
        => await DbSet
               .ToListAsync(cancellationToken)
               .ConfigureAwait(false);

    /// <inheritdoc />
    public virtual async Task UpdateAsync(TModel model, CancellationToken cancellationToken = default)
    {
        if (!model.IsTransient)
        {
            TModel? foundEntity = await GetByIdAsync(model.Id!, cancellationToken).ConfigureAwait(false);
            if (foundEntity is not null)
            {
                throw new NotFoundException($"SaveOrUpdate executed for an entity, id: {model.Id}, type: {model.GetType().FullName} that no longer exists in the database.");
            }
        }

        DbSet.Update(model);
    }

    /// <inheritdoc />
    public virtual void Delete(TModel model)
        => DbSet.Remove(model);

    public virtual DbSet<TModel> DbSet
        => _context.Set<TModel>();

    public virtual IQueryable<TModel> Queryable
        => DbSet;
}
