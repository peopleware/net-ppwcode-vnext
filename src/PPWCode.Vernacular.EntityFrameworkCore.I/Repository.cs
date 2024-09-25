using Microsoft.EntityFrameworkCore;

using PPWCode.Vernacular.Exceptions.V;
using PPWCode.Vernacular.Persistence.V;
using PPWCode.Vernacular.Persistence.V.Exceptions;

namespace PPWCode.Vernacular.EntityFrameworkCore.I;

/// <inheritdoc />
public abstract class Repository<TModel, TId> : IRepository<TModel, TId>
    where TModel : class, IPersistentObject<TId>, IIdentity<TId>
    where TId : IEquatable<TId>
{
    private readonly PpwDbContext _context;

    protected Repository(PpwDbContext dbContext)
    {
        _context = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
    }

    /// <inheritdoc cref="DbSet{TModel}.Find" />
    public virtual TModel? GetById(TId id)
        => DbSet.Find(id);

    /// <inheritdoc cref="DbSet{TModel}.FindAsync(object[])" />
    public virtual async Task<TModel?> GetByIdAsync(TId id, CancellationToken cancellationToken = default)
        => await DbSet
               .FindAsync([id], cancellationToken)
               .ConfigureAwait(false);

    /// <inheritdoc cref="IRepository{TModel,TId}.FindAll" />
    public virtual List<TModel> FindAll()
        => DbSet.ToList();

    /// <inheritdoc cref="IRepository{TModel,TId}.FindAllAsync" />
    public virtual async Task<List<TModel>> FindAllAsync(CancellationToken cancellationToken = default)
        => await DbSet
               .ToListAsync(cancellationToken)
               .ConfigureAwait(false);

    /// <inheritdoc cref="DbSet{TModel}.Update" />
    public virtual void Update(TModel model)
    {
        if (!model.IsTransient)
        {
            TModel? foundEntity = GetById(model.Id!);
            if (foundEntity is not null)
            {
                throw new IdNotFoundException<TModel, TId>(model.Id!);
            }
        }

        DbSet.Update(model);
    }

    /// <inheritdoc cref="DbSet{TModel}.Update" />
    public virtual async Task UpdateAsync(TModel model, CancellationToken cancellationToken = default)
    {
        if (!model.IsTransient)
        {
            TModel? foundEntity = await GetByIdAsync(model.Id!, cancellationToken).ConfigureAwait(false);
            if (foundEntity is not null)
            {
                throw new IdNotFoundException<TModel, TId>(model.Id!);
            }
        }

        DbSet.Update(model);
    }

    /// <inheritdoc cref="DbSet{TModel}.Add" />
    public virtual void Insert(TModel model)
    {
        if (!model.IsTransient)
        {
            throw new ProgrammingError("Adding a non-transient entity is not allowed.");
        }

        DbSet.Add(model);
    }

    /// <inheritdoc cref="DbSet{TModel}.AddRange(TModel[])" />
    public virtual async Task InsertAsync(TModel model, CancellationToken cancellationToken = default)
    {
        if (!model.IsTransient)
        {
            throw new ProgrammingError("Adding a non-transient entity is not allowed.");
        }

        await DbSet
            .AddAsync(model, cancellationToken)
            .ConfigureAwait(false);
    }

    /// <inheritdoc cref="DbSet{TEntity}.Remove" />
    public virtual void Delete(TModel model)
        => DbSet.Remove(model);

    /// <inheritdoc cref="DbSet{TEntity}.Remove" />
    public virtual Task DeleteAsync(TModel model, CancellationToken cancellationToken = default)
    {
        DbSet.Remove(model);

        return Task.CompletedTask;
    }

    /// <inheritdoc cref="DbContext.Set{TModel}()" />
    public virtual DbSet<TModel> DbSet
        => _context.Set<TModel>();

    /// <inheritdoc cref="DbSet{TModel}.AsQueryable" />
    public virtual IQueryable<TModel> Queryable
        => DbSet.AsQueryable();
}
