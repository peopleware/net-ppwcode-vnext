using PPWCode.Util.Time.I;
using PPWCode.Vernacular.Persistence.V;

namespace PPWCode.Vernacular.HistoryEvent.I;

/// <summary>
///     Interface that represents a store for events.
///     A new event can be registered using the <see cref="Open" /> method,
///     and an existing event can be closed using the
///     <see cref="Close" /> method.
/// </summary>
/// <typeparam name="TOwner">generic type of the owner</typeparam>
/// <typeparam name="TEvent">generic type of the event</typeparam>
/// <typeparam name="TId">generic idenyification type of TEvent</typeparam>
/// <typeparam name="TKnowledgePeriod">generic type of the knowledge period</typeparam>
/// <typeparam name="TKnowledge">generic type of the knowledge period members</typeparam>
/// <typeparam name="TContext">type of the context can be used to have extension points during processing</typeparam>
/// <typeparam name="TSelf">type of the implementation</typeparam>
public interface IHistoryEventStore<TOwner, TEvent, TId, TKnowledgePeriod, in TKnowledge, TContext, TSelf>
    where TEvent : IHistoryEvent<TKnowledgePeriod, TKnowledge, TOwner, TSelf>, IPersistentObject<TId>
    where TId : IEquatable<TId>
    where TKnowledgePeriod : Period<TKnowledge>, new()
    where TKnowledge : struct, IComparable<TKnowledge>, IEquatable<TKnowledge>
    where TContext : IHistoryEventStoreContext
    where TSelf : IHistoryEvent<TKnowledgePeriod, TKnowledge, TOwner, TSelf>
{
    /// <summary>
    ///     Registers the given event in the store, using the given transaction time.
    /// </summary>
    /// <param name="event">the new event, a transient object</param>
    /// <param name="transactionTime">the transaction time used for the knowledge dates</param>
    /// <param name="context">optional context of type <typeparamref name="TContext" /></param>
    /// <returns>the newly created event, which is no longer transient</returns>
    TEvent Open(TEvent @event, TKnowledge transactionTime, TContext? context = default);

    /// <summary>
    ///     Closes the given event in the store, using the given transaction time.
    /// </summary>
    /// <param name="event">the existing event, a non-transient object</param>
    /// <param name="transactionTime">the transaction time used for the knowledge dates</param>
    /// <param name="context">optional context of type <typeparamref name="TContext" /></param>
    /// <returns>the updated event</returns>
    TEvent Close(TEvent @event, TKnowledge transactionTime, TContext? context = default);

    /// <summary>
    ///     Processes the opened and closed events in this store.  This means that all newly created
    ///     events that do not have an empty knowledge period, will be created in the repository.
    ///     Events with an empty knowledge period, were created and closed in the same "session" and
    ///     these will be dropped.
    /// </summary>
    /// <param name="context">optional context of type <typeparamref name="TContext" /></param>
    /// <param name="onCreate">Optional lambda that will be invoked and should be responsible to save the event</param>
    /// <returns>
    ///     The method returns all events that were persisted, i.e. all events that were either
    ///     opened or closed, and did not have an empty knowledge period.
    /// </returns>
    /// <remarks>This method is only to be used during the migration!</remarks>
    ISet<TEvent> Process(TContext? context = default, Action<TEvent, TContext?>? onCreate = default);

    /// <summary>
    ///     Processes the opened and closed events in this store.  This means that all newly created
    ///     events that do not have an empty knowledge period, will be created in the repository.
    ///     Events with an empty knowledge period, were created and closed in the same "session" and
    ///     these will be dropped.
    /// </summary>
    /// <param name="transactionTime">use this transaction time as a reference, as the 'current' transaction time</param>
    /// <param name="context">optional context of type <typeparamref name="TContext" /></param>
    /// <param name="onCreate">Optional lambda that will be invoked and should be responsible to save the event</param>
    /// <returns>
    ///     The method returns all events that were persisted, i.e. all events that were either
    ///     opened or closed, and did not have an empty knowledge period.
    /// </returns>
    /// <remarks>This method is only to be used during the migration!</remarks>
    ISet<TEvent> Process(TKnowledge transactionTime, TContext? context = default, Action<TEvent, TContext?>? onCreate = default);
}
