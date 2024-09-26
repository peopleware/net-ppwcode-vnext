using PPWCode.Vernacular.Contracts.I;
using PPWCode.Vernacular.Persistence.V;
using PPWCode.Vernacular.RequestContext.I;
using PPWCode.Vernacular.Semantics.V;

namespace PPWCode.Vernacular.HistoryEvent.I;

/// <inheritdoc />
public abstract class HistoryEventStore<TOwner, TEvent, TId, T, TContext> : IHistoryEventStore<TOwner, TEvent, TId, T, TContext>
    where TEvent : IHistoryEvent<T, TOwner>, IPersistentObject<TId>
    where T : struct, IComparable<T>, IEquatable<T>
    where TContext : class, IHistoryEventContext<T>
    where TOwner : notnull
    where TId : IEquatable<TId>
{
    private readonly IDictionary<TOwner, ISet<TEvent>> _ownerEvents =
        new Dictionary<TOwner, ISet<TEvent>>();

    private readonly ISet<TOwner> _touchedOwners =
        new HashSet<TOwner>();

    private T? _currentTransactionTime;
    private T? _previousTransactionTime;

    protected HistoryEventStore(IRequestContext<T> requestContext)
    {
        RequestContext = requestContext;
    }

    public IRequestContext<T> RequestContext { get; }

    /// <inheritdoc />
    public virtual TEvent Open(TEvent @event, T transactionTime, TContext? context = default)
    {
        // when using the same event-store to process multiple transaction-times
        // each transaction-time should be the same or more recent than the previous one
        Contract.Requires(transactionTime.CompareTo(RequestContext.RequestTimestamp) <= 0);
        Contract.Requires(_previousTransactionTime is null || (_previousTransactionTime.Value.CompareTo(transactionTime) <= 0));
        Contract.Requires(_currentTransactionTime is null || _currentTransactionTime.Value.Equals(transactionTime));

        _currentTransactionTime ??= transactionTime;
        @event.SetKnowledgePeriod(transactionTime, null);
        StoreEvent(@event, context);

        return @event;
    }

    /// <inheritdoc />
    public virtual TEvent Close(TEvent @event, T transactionTime, TContext? context = default)
    {
        // when using the same event-store to process multiple transaction-times
        // each transaction-time should be the same or more recent than the previous one
        Contract.Requires(transactionTime.CompareTo(RequestContext.RequestTimestamp) <= 0);
        Contract.Requires(_previousTransactionTime is null || (_previousTransactionTime.Value.CompareTo(transactionTime) <= 0));
        Contract.Requires(_currentTransactionTime is null || _currentTransactionTime.Value.Equals(transactionTime));

        _currentTransactionTime ??= transactionTime;
        @event.SetKnowledgePeriod(@event.KnowledgePeriod.From, transactionTime);

        StoreEvent(@event, context);

        return @event;
    }

    /// <inheritdoc />
    public virtual ISet<TEvent> Process(TContext? context = default)
        => Process(RequestContext.RequestTimestamp, context);

    /// <inheritdoc />
    public virtual ISet<TEvent> Process(T transactionTime, TContext? context = default)
    {
        // when using the same event-store to process multiple transaction-times
        // each transaction-time should be the same or more recent than the previous one
        Contract.Requires(transactionTime.CompareTo(RequestContext.RequestTimestamp) <= 0);
        Contract.Requires(_previousTransactionTime is null || (_previousTransactionTime.Value.CompareTo(transactionTime) <= 0));
        Contract.Requires(_currentTransactionTime is null || _currentTransactionTime.Value.Equals(transactionTime));

        _currentTransactionTime ??= transactionTime;

        ISet<TEvent> result = new HashSet<TEvent>();

        // only visit touched roots
        foreach (TOwner owner in _touchedOwners)
        {
            // 1. Only civilized owners can be processed
            // 2. there must be events for this root
            // 3. if there is an empty-knowledge-period, it can only be on the current transaction-time
            Contract.Assert(owner is ICivilizedObject civilizedObject ? civilizedObject.IsCivilized : true);
            Contract.Assert(_ownerEvents.ContainsKey(owner));
            Contract.Assert(
                _ownerEvents[owner]
                    .All(e => !e.KnowledgePeriod.From.Equals(e.KnowledgePeriod.To)
                              || e.KnowledgePeriod.From.Equals(transactionTime)));

            // events-to-process should only contain events from the current transaction-time
            // the previous transaction-time could be the same as the current one,
            // but any events that are not on the current transaction-time must be older and can be ignored
            IEnumerable<TEvent> eventsToIgnore =
                _ownerEvents[owner]
                    .Where(e => !e.KnowledgePeriod.From.Equals(transactionTime)
                                && !e.KnowledgePeriod.To.Equals(transactionTime));
            _ownerEvents[owner].ExceptWith(eventsToIgnore);
            ISet<TEvent> events = _ownerEvents[owner];

            // remove events with an empty knowledge period [X, X[
            TEvent[] emptyKnowledgePeriods =
                events
                    .Where(e => e.KnowledgePeriod.From.Equals(e.KnowledgePeriod.To))
                    .ToArray();
            foreach (TEvent @event in emptyKnowledgePeriods)
            {
                if (!@event.IsTransient)
                {
                    Delete(@event, context);
                }

                events.Remove(@event);
            }

            // optimize identical events with consecutive knowledge periods
            ISet<TEvent> eventsToProcess = new HashSet<TEvent>(events);
            while (eventsToProcess.Count > 0)
            {
                // generate candidates with same properties and execution-period
                TEvent @event = eventsToProcess.First();
                TEvent[] candidates =
                    eventsToProcess
                        .Where(e => @event.HasIdenticalEventProperties(e) && e.HasIdenticalExecutionPeriod(e))
                        .OrderBy(e => e.KnowledgePeriod.From)
                        .ToArray();
                Contract.Assert(candidates.Length <= 2);

                // merge if consecutive and next if possible
                if (candidates.Length == 2)
                {
                    TEvent original = candidates[0];
                    TEvent candidate = candidates[1];

                    // if there are 2 candidates, the following should always be true,
                    // this must be true, since only the events related to transaction-time are processed
                    Contract.Assert(original.KnowledgePeriod.To.Equals(candidate.KnowledgePeriod.From));
                    Contract.Assert(candidate.KnowledgePeriod.From.Equals(transactionTime));
                    Contract.Assert(candidate.KnowledgePeriod.To == null);

                    original.SetKnowledgePeriod(original.KnowledgePeriod.From, null);

                    if (!candidate.IsTransient)
                    {
                        Delete(candidate, context);
                    }

                    events.Remove(candidate);
                    eventsToProcess.Remove(candidate);
                    eventsToProcess.Remove(original);
                    result.Add(original);
                }
                else
                {
                    Contract.Assert(candidates.Length == 1);
                    Contract.Assert(candidates[0].Equals(@event));

                    eventsToProcess.Remove(@event);
                    result.Add(@event);
                }
            }

            // Give the ability they do extra processing in our root object
            OnProcessRoot(owner, context);
        }

        OnProcessActualEvents(result, context);

        // save
        foreach (TEvent @event in result.Where(e => e.IsTransient))
        {
            Insert(@event, context);
        }

        // reset transaction time
        Contract.Assert(_currentTransactionTime != null);
        _previousTransactionTime = _currentTransactionTime;
        _currentTransactionTime = null;
        _touchedOwners.Clear();
        _ownerEvents.Clear();

        return result;
    }

    /// <summary>
    ///     Given an event of type <typeparamref name="TEvent" />, extract / determine a root object of type
    ///     <typeparamref name="TOwner" />
    /// </summary>
    /// <param name="event">Object where we extract an object of type <typeparamref name="TOwner" /></param>
    /// <returns>
    ///     An object of type <typeparamref name="TOwner" />
    /// </returns>
    protected virtual TOwner ExtractOwnerFrom(TEvent @event)
        => @event.Owner;

    /// <summary>
    ///     Give the ability to do some specific tasks, when we are processing a
    ///     <typeparam name="TOwner"> object</typeparam>
    /// </summary>
    /// <param name="owner">An  <typeparamref name="TOwner" object /></param>
    /// <param name="context">optional context of type <typeparamref name="TContext" /></param>
    protected abstract void OnProcessRoot(TOwner owner, TContext? context);

    /// <summary>
    ///     Give the ability to do some specific tasks given the <typeparamref name="TEvent" />s
    ///     before these <typeparamref name="TEvent" />s are persisted to the database
    /// </summary>
    /// <param name="events">The <typeparamref name="TEvent" />s to base the tasks on</param>
    /// <param name="context">optional context of type <typeparamref name="TContext" /></param>
    protected abstract void OnProcessActualEvents(ISet<TEvent> events, TContext? context);

    /// <summary>
    ///     Delete the <paramref name="event" /> in the data store.
    /// </summary>
    /// <remarks>The <paramref name="event"/> is guaranteed a non-transient entity</remarks>
    /// <param name="event">event to be deleted</param>
    /// <param name="context">optional context of type <typeparamref name="TContext" /></param>
    protected abstract void Delete(TEvent @event, TContext? context);

    /// <summary>
    ///     Add the <paramref name="event" /> in the data store.
    /// </summary>
    /// <remarks>The <paramref name="event"/> is guaranteed a transient entity</remarks>
    /// <param name="event">event to be added</param>
    /// <param name="context">optional context of type <typeparamref name="TContext" /></param>
    protected abstract void Insert(TEvent @event, TContext? context);

    private void StoreEvent(TEvent @event, TContext? context)
    {
        TOwner root = ExtractOwnerFrom(@event);

        if (!_ownerEvents.ContainsKey(root))
        {
            _ownerEvents[root] = new HashSet<TEvent>();
        }

        _ownerEvents[root].Add(@event);
        _touchedOwners.Add(root);
    }
}
