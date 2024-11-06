using PPWCode.Vernacular.Exceptions.V;
using PPWCode.Vernacular.Persistence.V.Exceptions;

namespace PPWCode.Vernacular.Persistence.V;

public interface IRepository<TModel, in TId>
    where TModel : IPersistentObject<TId>, IIdentity<TId>
    where TId : IEquatable<TId>
{
    /// <summary>
    ///     <para>
    ///         Finds an entity with the given primary key values. If an entity with the given primary key values
    ///         is being tracked by the underlying ORM, then it is returned immediately without making a request to the
    ///         database. Otherwise, a query is made to the database for an entity with the given primary key values
    ///         and this entity, if found, is attached to the context and returned. If no entity is found, then
    ///         null is returned.
    ///     </para>
    ///     <para>It is important to read the documentation of the used ORM, what exactly will be done.</para>
    /// </summary>
    /// <param name="id">The value of the primary key for the entity to be found.</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken" /> to observe while waiting for the task to complete.</param>
    /// <returns>The entity found, or <see langword="null" />.</returns>
    /// <exception cref="OperationCanceledException">If the <see cref="CancellationToken" /> is canceled.</exception>
    Task<TModel?> GetByIdAsync(TId id, CancellationToken cancellationToken = default);

    /// <summary>
    ///     <para>
    ///         Finds an entity with the given primary key values. If an entity with the given primary key values
    ///         is being tracked by the underlying ORM, then it is returned immediately without making a request to the
    ///         database. Otherwise, a query is made to the database for an entity with the given primary key values
    ///         and this entity, if found, is attached to the context and returned. If no entity is found, then
    ///         null is returned.
    ///     </para>
    ///     <para>It is important to read the documentation of the used ORM, what exactly will be done.</para>
    /// </summary>
    /// <param name="id">The value of the primary key for the entity to be found.</param>
    /// <returns>The entity found, or <see langword="null" />.</returns>
    TModel? GetById(TId id);

    /// <summary>
    ///     Find all entities of <typeparamref name="TModel" />.
    /// </summary>
    /// <param name="cancellationToken">A <see cref="CancellationToken" /> to observe while waiting for the task to complete.</param>
    /// <returns>All entities of <typeparamref name="TModel" /></returns>
    /// <exception cref="OperationCanceledException">If the <see cref="CancellationToken" /> is canceled.</exception>
    Task<List<TModel>> FindAllAsync(CancellationToken cancellationToken = default);

    /// <summary>
    ///     Find all entities of <typeparamref name="TModel" />.
    /// </summary>
    /// <returns>All entities of <typeparamref name="TModel" /></returns>
    List<TModel> FindAll();

    /// <summary>
    ///     <para>Update the entity.</para>
    ///     <para>
    ///         Some ORM's just mark the entity and flush their db context, others directly create an update
    ///         statement against the data store.
    ///     </para>
    ///     <para>It is important to read the documentation of the used ORM, what exactly will be done.</para>
    /// </summary>
    /// <param name="model">The entity to update.</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken" /> to observe while waiting for the task to complete.</param>
    /// <returns>
    ///     an awaitable <see cref="Task" />
    /// </returns>
    /// <exception cref="OperationCanceledException">If the <see cref="CancellationToken" /> is canceled.</exception>
    /// <exception cref="IdNotFoundException{T,TId}">If the <paramref name="model" /> is transient.</exception>
    Task UpdateAsync(TModel model, CancellationToken cancellationToken = default);

    /// <summary>
    ///     <para>Update the entity.</para>
    ///     <para>
    ///         Some ORM's just mark the entity and flush their db context, others directly create an update
    ///         statement against the data store.
    ///     </para>
    ///     <para>It is important to read the documentation of the used ORM, what exactly will be done.</para>
    /// </summary>
    /// <param name="model">The entity to update.</param>
    /// <exception cref="IdNotFoundException{T,TId}">If the <paramref name="model" /> is transient.</exception>
    void Update(TModel model);

    /// <summary>
    ///     <para>Make the entity non-transient, make it persistent in the data store.</para>
    ///     <para>
    ///         Dependent on the used ORM, the entity can be sent to the data store or not. The generation of
    ///         the primary key is the crucial factor. The entity can also be persisted in the data store when their session /
    ///         context is being flushed.
    ///     </para>
    ///     <para>It is important to read the documentation of the used ORM, what exactly will be done.</para>
    /// </summary>
    /// <param name="model">The entity to update.</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken" /> to observe while waiting for the task to complete.</param>
    /// <returns>
    ///     an awaitable <see cref="Task" />
    /// </returns>
    /// <exception cref="OperationCanceledException">If the <see cref="CancellationToken" /> is canceled.</exception>
    /// <exception cref="ProgrammingError">If the <paramref name="model" /> is non-transient.</exception>
    Task InsertAsync(TModel model, CancellationToken cancellationToken = default);

    /// <summary>
    ///     <para>Make the entity non-transient, make it persistent in the data store.</para>
    ///     <para>
    ///         Dependent on the used ORM, the entity can be sent to the data store or not. The generation of
    ///         the primary key is the crucial factor. The entity can also be persisted in the data store when their session /
    ///         context is being flushed.
    ///     </para>
    ///     <para>It is important to read the documentation of the used ORM, what exactly will be done.</para>
    /// </summary>
    /// <param name="model">The entity to update.</param>
    /// <exception cref="ProgrammingError">If the <paramref name="model" /> is non-transient.</exception>
    void Insert(TModel model);

    /// <summary>
    ///     <para>Remove the entity from the data store</para>
    ///     <para>
    ///         Dependent on the ORM mapper the deletion can be a NOP. If the ORM doesn't send a newly added transient entity
    ///         to the data store immediately, but afterward, a deletion is being asked.
    ///     </para>
    ///     <para>It is important to read the documentation of the used ORM, what exactly will be done.</para>
    /// </summary>
    /// <param name="model">The entity to update.</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken" /> to observe while waiting for the task to complete.</param>
    /// <returns>
    ///     an awaitable <see cref="Task" />
    /// </returns>
    /// <exception cref="OperationCanceledException">If the <see cref="CancellationToken" /> is canceled.</exception>
    Task DeleteAsync(TModel model, CancellationToken cancellationToken = default);

    /// <summary>
    ///     <para>Remove the entity from the data store</para>
    ///     <para>
    ///         Dependent on the ORM mapper the deletion can be a NOP. If the ORM doesn't send a newly added transient entity
    ///         to the data store immediately, but afterward, a deletion is being asked.
    ///     </para>
    ///     <para>It is important to read the documentation of the used ORM, what exactly will be done.</para>
    /// </summary>
    /// <param name="model">The entity to update.</param>
    void Delete(TModel model);

    /// <summary>
    ///     An entity (or model) is considered transient if it has been created but not yet saved to the database.
    /// </summary>
    /// <param name="model">The entity for which to update.</param>
    /// <returns><c>true</c> if transient.</returns>
    bool IsTransient(TModel model);
}
