﻿// Copyright 2024 by PeopleWare n.v..
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// http://www.apache.org/licenses/LICENSE-2.0
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using PPWCode.Vernacular.Exceptions.V;

namespace PPWCode.Vernacular.Persistence.V.Tests;

/// <summary>
///     This class is a base class for in-memory repositories that can be used in tests.
/// </summary>
/// <remarks>
///     <para>
///         The use of 2 generic type parameters might seem to overkill at first sight, but this is necessary in the
///         context
///         of subclass hierarchies of models.  In that case, typically a generic base repository is created that works
///         on a common superclass, and more specific repositories are derived from that to work on the subclasses.
///     </para>
///     <para>
///         Each repository uses a given list for its storage.  In the context of subclass hierarchies, it's
///         important that the whole hierarchy is stored inside the same list.  If that is not the case, it would
///         cause weird behavior.
///     </para>
///     <para>
///         For example: Suppose that we have a super-class S and subclass A. This would result in a generic
///         repository on S and a specialized repository for A.  Now, suppose that in a test, an instance of A is
///         initially created and "save"d using the specialized repository for A.  Then further on in the test,
///         the repository on S is used to retrieve all existing instances.  In that case, the correct behavior is
///         that the newly created instance of A is returned.  To get this behavior, it is really important that
///         all in-memory repositories in the same subclass hierarchy are initialized with the same instance of the
///         list that is used for storage.  And that is only possible if that list is of the superclass (in this
///         case S).
///     </para>
///     <para>
///         So that is the reason for introducing 2 generic type parameters. <typeparamref name="TModel" /> is a
///         subclass of <typeparamref name="TBase" />.  The underlying storage is of the type
///         <typeparamref name="TBase" />.  All methods on the repository work in the context of type
///         <typeparamref name="TModel" />.
///     </para>
///     <para>
///         Important notes regarding the implementation.  The protected property <see cref="Models" /> returns a list of
///         all
///         instances in storage of the type <typeparamref name="TModel" />, and should be used for all "querying"
///         operations. The protected property <see cref="BaseModels" /> gives access to the underlying storage and
///         should be used for create and delete operations.
///     </para>
/// </remarks>
/// <typeparam name="TBase">the base type of <typeparamref name="TModel" /></typeparam>
/// <typeparam name="TModel">the type that this repository works on</typeparam>
/// <typeparam name="TId">the type of the PK of <typeparamref name="TModel" /></typeparam>
public abstract class InMemoryRepository<TBase, TModel, TId> : IRepository<TModel, TId>
    where TBase : IPersistentObject<TId>
    where TModel : TBase
    where TId : struct, IEquatable<TId>
{
    protected InMemoryRepository(List<TBase> baseModels)
    {
        BaseModels = baseModels;
    }

    /// <summary>
    ///     Returns the raw storage based on type <typeparamref name="TBase" /> and is meant for manipulation:
    ///     adding or removing instances.
    /// </summary>
    protected List<TBase> BaseModels { get; }

    /// <summary>
    ///     Returns all instances that can be "seen" by this repository and is based on type
    ///     <typeparamref name="TModel" />.  This is meant for querying data.
    /// </summary>
    public List<TModel> Models
        => BaseModels
            .OfType<TModel>()
            .ToList();

    /// <summary>
    ///     Returns all instances that can be "seen" by this repository and is based on type
    ///     <typeparamref name="TModel" />.  This is meant for querying data.
    /// </summary>
    protected IQueryable<TModel> Queryable
        => Models.AsQueryable();

    public virtual void Initialize(IEnumerable<TBase> models)
    {
        foreach (TBase model in models)
        {
            if (!model.IsCivilized)
            {
                CompoundSemanticException cse = model.WildExceptions();
                throw new ProgrammingError("model should be civilized", cse);
            }

            if (model.IdIsTransient)
            {
                SetIdAndCreateAuditProperties(model, GetNextIdFor(model));
            }
        }

        BaseModels.RemoveAll(m => m is TModel);
        BaseModels.AddRange(models);
    }

    /// <inheritdoc />
    public virtual Task<TModel?> GetByIdAsync(TId id, CancellationToken cancellationToken = default)
        => Task.FromResult(Models.SingleOrDefault(m => m.Id.Equals(id)));

    /// <inheritdoc />
    public TModel? GetById(TId id)
        => GetByIdAsync(id)
            .ConfigureAwait(false)
            .GetAwaiter()
            .GetResult();

    /// <inheritdoc />
    public virtual Task<List<TModel>> FindAllAsync(CancellationToken cancellationToken = default)
        => Task.FromResult(Models.ToList());

    /// <inheritdoc />
    public List<TModel> FindAll()
        => FindAllAsync()
            .ConfigureAwait(false)
            .GetAwaiter()
            .GetResult();

    /// <inheritdoc />
    public virtual async Task UpdateAsync(TModel model, CancellationToken cancellationToken = default)
    {
        // Is the entity civilized? If not, we do not perform an 'update'.
        model.ThrowIfNotCivilized();
        if (model.IdIsTransient)
        {
            throw new ProgrammingError("model cannot be transient.");
        }

        TModel? entity = await GetByIdAsync(model.Id, cancellationToken);
        if (entity == null)
        {
            throw new InternalProgrammingError("Attempting to update an entity with a identifier not known by the repository");
        }

        if (!ReferenceEquals(entity, model))
        {
            throw new InternalProgrammingError("Attempting to update the reference of an existing entity.");
        }

        SetLastModifiedProperties(model);
    }

    /// <inheritdoc />
    public void Update(TModel model)
        => UpdateAsync(model)
            .ConfigureAwait(false)
            .GetAwaiter()
            .GetResult();

    /// <inheritdoc />
    public virtual Task InsertAsync(TModel model, CancellationToken cancellationToken = default)
    {
        // Is the entity civilized? If not, we do not perform an 'update'.
        model.ThrowIfNotCivilized();
        if (!model.IdIsTransient)
        {
            throw new ProgrammingError("model should be transient.");
        }

        SetIdAndCreateAuditProperties(model, GetNextIdFor(model));
        BaseModels.Add(model);

        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public void Insert(TModel model)
        => InsertAsync(model)
            .ConfigureAwait(false)
            .GetAwaiter()
            .GetResult();

    /// <inheritdoc />
    public virtual Task DeleteAsync(TModel model, CancellationToken cancellationToken = default)
    {
        BaseModels.Remove(model);

        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public void Delete(TModel model)
        => DeleteAsync(model)
            .ConfigureAwait(false)
            .GetAwaiter()
            .GetResult();

    /// <inheritdoc />
    public virtual bool IsTransient(TModel model)
        => model.IdIsTransient;

    /// <summary>
    ///     <para>
    ///         Find a number of tuples and count them.
    ///     </para>
    ///     <para>The <paramref name="lambda" /> describes the predicate to be used.</para>
    /// </summary>
    /// <param name="lambda">The predicate to be used to query our store</param>
    /// <typeparam name="TResult">
    ///     The <see cref="Type" /> of the result. It doesn't need to be of type
    ///     <typeparamref name="TModel" />.
    /// </typeparam>
    /// <param name="cancellationToken">A <see cref="CancellationToken" /> to observe while waiting for the task to complete.</param>
    /// <returns>
    ///     an awaitable <see cref="Task" />
    /// </returns>
    /// <exception cref="OperationCanceledException">If the <see cref="CancellationToken" /> is canceled.</exception>
    public virtual Task<int> CountAsync<TResult>(Func<IQueryable<TModel>, IQueryable<TResult>> lambda, CancellationToken cancellationToken = default)
        => Task.FromResult(lambda(Queryable).Count());

    /// <summary>
    ///     <para>
    ///         Find a number of tuples and count them.
    ///     </para>
    ///     <para>The <paramref name="lambda" /> describes the predicate to be used.</para>
    /// </summary>
    /// <param name="lambda">The predicate to be used to query our store</param>
    /// <typeparam name="TResult">
    ///     The <see cref="Type" /> of the result. It doesn't need to be of type
    ///     <typeparamref name="TModel" />.
    /// </typeparam>
    /// <returns>
    ///     The count of tuples
    /// </returns>
    public int Count<TResult>(Func<IQueryable<TModel>, IQueryable<TResult>> lambda)
        => CountAsync(lambda)
            .ConfigureAwait(false)
            .GetAwaiter()
            .GetResult();

    /// <summary>
    ///     <para>
    ///         Find a tuple at <paramref name="index" />.
    ///     </para>
    ///     <para>The <paramref name="lambda" /> describes the predicate to be used.</para>
    /// </summary>
    /// <param name="lambda">The predicate to be used to query our store</param>
    /// <param name="index">The index of the tuple to be returned</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken" /> to observe while waiting for the task to complete.</param>
    /// <typeparam name="TResult">
    ///     The <see cref="Type" /> of the result. It doesn't need to be of type
    ///     <typeparamref name="TModel" />.
    /// </typeparam>
    /// <returns>
    ///     an awaitable <see cref="Task" />
    /// </returns>
    /// <exception cref="OperationCanceledException">If the <see cref="CancellationToken" /> is canceled.</exception>
    /// <remarks>
    ///     Remember to specify an order, otherwise the result isn't deterministic.
    /// </remarks>
    protected virtual Task<TResult?> GetAtIndexAsync<TResult>(Func<IQueryable<TModel>, IQueryable<TResult>> lambda, int index, CancellationToken cancellationToken = default)
        => Task.FromResult(
            lambda(Queryable)
                .Skip(index)
                .Take(1)
                .SingleOrDefault());

    /// <summary>
    ///     <para>
    ///         Find a tuple at <paramref name="index" />.
    ///     </para>
    ///     <para>The <paramref name="lambda" /> describes the predicate to be used.</para>
    /// </summary>
    /// <param name="lambda">The predicate to be used to query our store</param>
    /// <param name="index">The index of the tuple to be returned</param>
    /// <typeparam name="TResult">
    ///     The <see cref="Type" /> of the result. It doesn't need to be of type
    ///     <typeparamref name="TModel" />.
    /// </typeparam>
    /// <returns>
    ///     The tuple at <paramref name="index" /> of the tuples that satisfied the <paramref name="lambda" />.
    /// </returns>
    /// <remarks>
    ///     Remember to specify an order, otherwise the result isn't deterministic.
    /// </remarks>
    protected TResult? GetAtIndex<TResult>(Func<IQueryable<TModel>, IQueryable<TResult>> lambda, int index)
        => GetAtIndexAsync(lambda, index)
            .ConfigureAwait(false)
            .GetAwaiter()
            .GetResult();

    /// <summary>
    ///     <para>
    ///         Find a specified range of contiguous tuples, defined by <paramref name="take" />, after skipping
    ///         a number of tuples, defined by <paramref name="skip" />.
    ///     </para>
    ///     <para>The <paramref name="lambda" /> describes the predicate to be used.</para>
    /// </summary>
    /// <param name="lambda">The predicate to be used to query our store</param>
    /// <param name="skip">The number of tuples to skip before returning the remaining tuples</param>
    /// <param name="take">Returns a specified range of contiguous tuples from a sequence</param>
    /// <typeparam name="TResult">
    ///     The <see cref="Type" /> of the result. It doesn't need to be of type
    ///     <typeparamref name="TModel" />.
    /// </typeparam>
    /// <param name="cancellationToken">A <see cref="CancellationToken" /> to observe while waiting for the task to complete.</param>
    /// <returns>
    ///     an awaitable <see cref="Task" />
    /// </returns>
    /// <remarks>
    ///     Remember to specify an order, otherwise the result isn't deterministic.
    /// </remarks>
    /// <exception cref="OperationCanceledException">If the <see cref="CancellationToken" /> is canceled.</exception>
    protected virtual Task<List<TResult>> FindAsync<TResult>(
        Func<IQueryable<TModel>, IQueryable<TResult>> lambda,
        int? skip = null,
        int? take = null,
        CancellationToken cancellationToken = default)
    {
        IQueryable<TResult> result = lambda(Queryable);
        if (skip != null)
        {
            result = result.Skip(skip.Value);
        }

        if (take != null)
        {
            result = result.Take(take.Value);
        }

        return Task.FromResult(result.ToList());
    }

    /// <summary>
    ///     <para>
    ///         Find a specified range of contiguous tuples, defined by <paramref name="take" />, after skipping
    ///         a number of tuples, defined by <paramref name="skip" />.
    ///     </para>
    ///     <para>The <paramref name="lambda" /> describes the predicate to be used.</para>
    /// </summary>
    /// <param name="lambda">The predicate to be used to query our store</param>
    /// <param name="skip">The number of tuples to skip before returning the remaining tuples</param>
    /// <param name="take">Returns a specified range of contiguous tuples from a sequence</param>
    /// <typeparam name="TResult">
    ///     The <see cref="Type" /> of the result. It doesn't need to be of type
    ///     <typeparamref name="TModel" />.
    /// </typeparam>
    /// <returns>
    ///     A list of tuples of type <typeparamref name="TResult" />
    /// </returns>
    /// <remarks>
    ///     Remember to specify an order, otherwise the result isn't deterministic.
    /// </remarks>
    protected List<TResult> Find<TResult>(Func<IQueryable<TModel>, IQueryable<TResult>> lambda, int? skip = null, int? take = null)
        => FindAsync(lambda, skip, take)
            .ConfigureAwait(false)
            .GetAwaiter()
            .GetResult();

    /// <summary>
    ///     <para>
    ///         Find a specified range of contiguous tuples, called a page defined by <paramref name="pageSize" />, after
    ///         skipping
    ///         a number of pages defined by <paramref name="pageIndex" />.
    ///     </para>
    ///     <para>The <paramref name="lambda" /> describes the predicate to be used.</para>
    /// </summary>
    /// <param name="lambda">The predicate to be used to query our store</param>
    /// <param name="pageIndex">The number of pages to skip before returning the remaining tuples</param>
    /// <param name="pageSize">Returns a specified range of contiguous tuples from a sequence, called a page</param>
    /// <typeparam name="TResult">
    ///     The <see cref="Type" /> of the result. It doesn't need to be of type
    ///     <typeparamref name="TModel" />.
    /// </typeparam>
    /// <param name="cancellationToken">A <see cref="CancellationToken" /> to observe while waiting for the task to complete.</param>
    /// <returns>
    ///     an awaitable <see cref="Task" />
    /// </returns>
    /// <remarks>
    ///     Remember to specify an order, otherwise the result isn't deterministic.
    /// </remarks>
    /// <exception cref="OperationCanceledException">If the <see cref="CancellationToken" /> is canceled.</exception>
    public virtual Task<IPagedList<TResult>> FindPagedAsync<TResult>(
        Func<IQueryable<TModel>, IQueryable<TResult>> lambda,
        int pageIndex,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        IQueryable<TResult> query = lambda(Queryable).Skip((pageIndex - 1) * pageSize).Take(pageSize);
        int count = lambda(Queryable).Count();

        return Task.FromResult<IPagedList<TResult>>(new PagedList<TResult>(query, pageIndex, pageSize, count));
    }

    /// <summary>
    ///     <para>
    ///         Find a specified range of contiguous tuples, called a page defined by <paramref name="pageSize" />, after
    ///         skipping
    ///         a number of pages defined by <paramref name="pageIndex" />.
    ///     </para>
    ///     <para>The <paramref name="lambda" /> describes the predicate to be used.</para>
    /// </summary>
    /// <param name="lambda">The predicate to be used to query our store</param>
    /// <param name="pageIndex">The number of pages to skip before returning the remaining tuples</param>
    /// <param name="pageSize">Returns a specified range of contiguous tuples from a sequence, called a page</param>
    /// <typeparam name="TResult">
    ///     The <see cref="Type" /> of the result. It doesn't need to be of type
    ///     <typeparamref name="TModel" />.
    /// </typeparam>
    /// <returns>
    ///     A page is returned, a page is defined using <see cref="IPagedList{T}" />
    /// </returns>
    /// <remarks>
    ///     Remember to specify an order, otherwise the result isn't deterministic.
    /// </remarks>
    protected IPagedList<TResult> FindPaged<TResult>(
        Func<IQueryable<TModel>, IQueryable<TResult>> lambda,
        int pageIndex,
        int pageSize)
        => FindPagedAsync(lambda, pageIndex, pageSize)
            .ConfigureAwait(false)
            .GetAwaiter()
            .GetResult();

    protected abstract TId GetNextIdFor(TBase model);
    protected abstract void SetIdAndCreateAuditProperties(TBase model, TId id);
    protected abstract void SetLastModifiedProperties(TBase model);

    protected virtual Task<TResult?> GetAsync<TResult>(Func<IQueryable<TModel>, IQueryable<TResult>> lambda, CancellationToken cancellationToken = default)
        => Task.FromResult(lambda(Queryable).SingleOrDefault());

    protected TResult? Get<TResult>(Func<IQueryable<TModel>, IQueryable<TResult>> lambda)
        => GetAsync(lambda)
            .ConfigureAwait(false)
            .GetAwaiter()
            .GetResult();

    protected virtual Task<TResult?> MaxAsync<TResult>(Func<IQueryable<TModel>, IQueryable<TResult>> lambda, CancellationToken cancellationToken = default)
        => Task.FromResult(lambda(Queryable).Max());

    protected TResult? Max<TResult>(Func<IQueryable<TModel>, IQueryable<TResult>> lambda)
        => MaxAsync(lambda)
            .ConfigureAwait(false)
            .GetAwaiter()
            .GetResult();

    protected TResult? Min<TResult>(Func<IQueryable<TModel>, IQueryable<TResult>> lambda)
        => MinAsync(lambda)
            .ConfigureAwait(false)
            .GetAwaiter()
            .GetResult();

    protected Task<TResult?> MinAsync<TResult>(Func<IQueryable<TModel>, IQueryable<TResult>> lambda, CancellationToken cancellationToken = default)
        => Task.FromResult(lambda(Queryable).Min());
}
