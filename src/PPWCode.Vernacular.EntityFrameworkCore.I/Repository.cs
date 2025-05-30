// Copyright 2025 by PeopleWare n.v..
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// http://www.apache.org/licenses/LICENSE-2.0
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using Microsoft.EntityFrameworkCore;

using PPWCode.Vernacular.Exceptions.V;
using PPWCode.Vernacular.Persistence.V;
using PPWCode.Vernacular.Persistence.V.Exceptions;

namespace PPWCode.Vernacular.EntityFrameworkCore.I;

/// <inheritdoc />
public abstract class Repository<TModel, TId, TTimestamp> : IRepository<TModel, TId>
    where TModel : class, IPersistentObject<TId>, IIdentity<TId>
    where TId : IEquatable<TId>
    where TTimestamp : struct, IComparable<TTimestamp>, IEquatable<TTimestamp>
{
    private readonly PpwDbContext<TTimestamp> _context;

    protected Repository(PpwDbContext<TTimestamp> dbContext)
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
        if (!model.IdIsTransient)
        {
            TModel? foundEntity = GetById(model.Id!);
            if (foundEntity is null)
            {
                throw new IdNotFoundException<TModel, TId>(model.Id!);
            }
        }

        DbSet.Update(model);
    }

    /// <inheritdoc cref="DbSet{TModel}.Update" />
    public virtual async Task UpdateAsync(TModel model, CancellationToken cancellationToken = default)
    {
        if (!model.IdIsTransient)
        {
            TModel? foundEntity = await GetByIdAsync(model.Id!, cancellationToken).ConfigureAwait(false);
            if (foundEntity is null)
            {
                throw new IdNotFoundException<TModel, TId>(model.Id!);
            }
        }

        DbSet.Update(model);
    }

    /// <inheritdoc cref="DbSet{TModel}.Add" />
    public virtual void Insert(TModel model)
    {
        if (!model.IdIsTransient)
        {
            throw new ProgrammingError("Adding a non-transient entity is not allowed.");
        }

        DbSet.Add(model);
    }

    /// <inheritdoc cref="DbSet{TModel}.AddRange(TModel[])" />
    public virtual async Task InsertAsync(TModel model, CancellationToken cancellationToken = default)
    {
        if (!model.IdIsTransient)
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

    /// <inheritdoc cref="IRepository{TModel,TId}.IsTransient" />
    public virtual bool IsTransient(TModel model)
        => (_context.Entry(model).State == EntityState.Detached) && model.IdIsTransient;

    /// <inheritdoc cref="DbContext.Set{TModel}()" />
    public virtual DbSet<TModel> DbSet
        => _context.Set<TModel>();

    /// <inheritdoc cref="DbSet{TModel}.AsQueryable" />
    public virtual IQueryable<TModel> Queryable
        => DbSet.AsQueryable();
}
