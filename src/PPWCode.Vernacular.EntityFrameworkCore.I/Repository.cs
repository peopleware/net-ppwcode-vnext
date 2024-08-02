// Copyright 2024 by PeopleWare n.v..
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
    public async Task<TModel?> GetByIdAsync(TId id, CancellationToken cancellationToken = default)
        => await _context
               .GetDbSet<TModel, TId>()
               .FindAsync(id, cancellationToken)
               .ConfigureAwait(false);

    /// <inheritdoc />
    public async Task<IList<TModel>> FindAllAsync(CancellationToken cancellationToken = default)
        => await _context
               .GetDbSet<TModel, TId>()
               .ToListAsync(cancellationToken: cancellationToken)
               .ConfigureAwait(false);

    /// <inheritdoc />
    public async Task UpdateAsync(TModel model, CancellationToken cancellationToken = default)
    {
        if (!model.IsTransient)
        {
            TModel? foundEntity = await GetByIdAsync(model.Id!, cancellationToken).ConfigureAwait(false);
            if (foundEntity is not null)
            {
                throw new NotFoundException($"SaveOrUpdate executed for an entity, id: {model.Id}, type: {model.GetType().FullName} that no longer exists in the database.");
            }
        }

        _context
            .GetDbSet<TModel, TId>()
            .Update(model);
    }

    /// <inheritdoc />
    public void Delete(TModel model)
        => _context
            .GetDbSet<TModel, TId>()
            .Remove(model);
}
