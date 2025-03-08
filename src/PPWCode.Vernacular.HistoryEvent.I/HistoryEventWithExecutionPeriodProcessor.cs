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

using PPWCode.Util.Time.I;
using PPWCode.Vernacular.Exceptions.V;
using PPWCode.Vernacular.Persistence.V;
using PPWCode.Vernacular.Persistence.V.Exceptions;
using PPWCode.Vernacular.RequestContext.I;

namespace PPWCode.Vernacular.HistoryEvent.I;

/// <inheritdoc cref="IHistoryEventWithExecutionPeriodProcessor{TOwner,TSubEvent,TId,TKnowledgePeriod,TKnowledge,TExecutionPeriod,TExecution,TEvent,THistoryEventStoreContext,TReferenceHistory,TPermissionHistory}" />
public abstract class HistoryEventWithExecutionPeriodProcessor<TOwner, TSubEvent, TId, TKnowledgePeriod, TKnowledge, TExecutionPeriod, TExecution, TEvent, THistoryEventStoreContext, TReferenceHistory, TPermissionHistory>
    : IHistoryEventWithExecutionPeriodProcessor<TOwner, TSubEvent, TId, TKnowledgePeriod, TKnowledge, TExecutionPeriod, TExecution, TEvent, THistoryEventStoreContext, TReferenceHistory, TPermissionHistory>
    where TEvent : IHistoryEvent<TKnowledgePeriod, TKnowledge, TOwner, TEvent>, IPersistentObject<TId>
    where TId : IEquatable<TId>
    where TKnowledgePeriod : Period<TKnowledge>, new()
    where TKnowledge : struct, IComparable<TKnowledge>, IEquatable<TKnowledge>
    where TExecutionPeriod : Period<TExecution>, new()
    where TExecution : struct, IComparable<TExecution>, IEquatable<TExecution>
    where TSubEvent : class, TEvent, IExecutionPeriod<TExecutionPeriod, TExecution>, new()
    where THistoryEventStoreContext : IHistoryEventStoreContext
    where TReferenceHistory : PeriodHistory<TExecutionPeriod, TExecution>
    where TPermissionHistory : PeriodHistory<TExecutionPeriod, TExecution>
{
    private static readonly TExecutionPeriod _infinitiveExecutionPeriod = new () { From = null, To = null };
    private readonly Stack<TPermissionHistory?> _permissionHistoryStack = new ();
    private TPermissionHistory? _permissionHistory;

    protected HistoryEventWithExecutionPeriodProcessor(
        IRequestContext<TKnowledge> requestContext,
        IHistoryEventStore<TOwner, TSubEvent, TId, TKnowledgePeriod, TKnowledge, THistoryEventStoreContext, TEvent> eventStore,
        HistoryEventProcessorContext<TOwner, TSubEvent, TId, TKnowledgePeriod, TKnowledge, TExecutionPeriod, TExecution, TEvent, THistoryEventStoreContext, TReferenceHistory, TPermissionHistory> eventProcessorContext)
    {
        TransactionTime = eventProcessorContext.TransactionTime ?? requestContext.RequestTimestamp;
        EventStore = eventStore;
        EventProcessorContext = eventProcessorContext;

        Events = eventProcessorContext.Events.ToList();
        _permissionHistory = eventProcessorContext.PermissionHistory;
        ReferenceHistory = eventProcessorContext.ReferenceHistory;
        HistoryEventStoreContext = eventProcessorContext.HistoryEventStoreContext;

        // events must be civilized
        if (Events.Any(e => !e.IsCivilized))
        {
            throw new InternalProgrammingError("Invalid history: all events must be civilized!");
        }

        // events should have knowledge- and execution periods, normally this should be done checking if an object is civilized
        if (Events.Any(e => e.KnowledgePeriod is null || e.ExecutionPeriod is null))
        {
            throw new InternalProgrammingError("Invalid history: knowledge / execution periods should be known!");
        }

        // * events must be consistent in their KnowledgePeriod with the TransactionTime
        // * TransactionTime must be in the future for every event
        // * events are civilized, so no empty KnowledgePeriod and ExecutionPeriod, therefor we can use the force not null marker: !
        if (Events.Any(e => (TransactionTime.CompareTo(e.KnowledgePeriod!.CoalesceFrom) < 0)
                            || (e.KnowledgePeriod.To is not null && (TransactionTime.CompareTo(e.KnowledgePeriod.CoalesceTo) < 0))))
        {
            throw new InternalProgrammingError("Invalid history: event knowledge dates cannot be in the future relative to transaction time!");
        }

        // no intersections, no overlaps
        LinkedList<TSubEvent> list = new (ActualEvents);
        LinkedListNode<TSubEvent>? node = list.First;
        while (node != null)
        {
            // current-from must be greater or equal than previous-to
            if ((node.Previous != null)
                && (node.Value.ExecutionPeriod!.CoalesceFrom.CompareTo(node.Previous.Value.ExecutionPeriod!.CoalesceTo) < 0))
            {
                throw new InternalProgrammingError("Invalid history: overlapping intervals!");
            }

            node = node.Next;
        }
    }

    /// <summary>
    ///     The store that can be used to create new events and close existing events.
    /// </summary>
    public IHistoryEventStore<TOwner, TSubEvent, TId, TKnowledgePeriod, TKnowledge, THistoryEventStoreContext, TEvent> EventStore { get; }

    public THistoryEventStoreContext? HistoryEventStoreContext { get; }

    /// <inheritdoc />
    public IList<TSubEvent> Events { get; }

    /// <inheritdoc />
    public HistoryEventProcessorContext<TOwner, TSubEvent, TId, TKnowledgePeriod, TKnowledge, TExecutionPeriod, TExecution, TEvent, THistoryEventStoreContext, TReferenceHistory, TPermissionHistory> EventProcessorContext { get; }

    /// <inheritdoc />
    public TReferenceHistory? ReferenceHistory { get; }

    /// <inheritdoc />
    public TPermissionHistory? PermissionHistory
        => _permissionHistory;

    /// <inheritdoc />
    public TKnowledge TransactionTime { get; }

    /// <inheritdoc />
    public IList<TSubEvent> ActualEvents
        => Events
            .Where(e => e.KnowledgePeriod!.To == null)
            .OrderBy(e => e.ExecutionPeriod!.CoalesceFrom)
            .ToList();

    /// <inheritdoc />
    public IList<TExecutionPeriod> ActualPeriods
        => ActualEvents
            .Select(e => e.ExecutionPeriod!)
            .ToList();

    /// <inheritdoc />
    public TSubEvent? GetActualEventAt(TExecution date)
        => ActualEvents
            .SingleOrDefault(e => e.ExecutionPeriod!.Contains(date));

    /// <inheritdoc />
    public void Create(TSubEvent historyEvent)
    {
        if (!EventStore.IsTransient(historyEvent, HistoryEventStoreContext))
        {
            throw new InternalProgrammingError("Create can only be called with a transient event!");
        }

        // override the knowledge period, to guarantee transaction time
        historyEvent.KnowledgePeriod = new TKnowledgePeriod { From = TransactionTime, To = null };

        // helper variables
        TExecution? from = historyEvent.ExecutionPeriod!.From;
        TExecution? to = historyEvent.ExecutionPeriod.To;

        // denormalized intervals
        List<TExecutionPeriod> denormalizedIntervals = CalculateDenormalizedIntervals(from, to);

        // denormalized events, semantically the same as actual-events
        List<TSubEvent> denormalizedEvents = BuildDenormalizedEvents(denormalizedIntervals);

        // operation events, denormalized

        // execute change
        TExecutionPeriod newPeriod = historyEvent.ExecutionPeriod!;
        List<TSubEvent> updatedDenormalizedEvents = new ();

        void CreateNewEventOnInterval(TExecutionPeriod p)
        {
            // make clone with new values
            TSubEvent clone = CloneEvent(historyEvent);

            // fix execution period
            clone.ExecutionPeriod = new TExecutionPeriod { From = p.From, To = p.To };

            // add in the list
            updatedDenormalizedEvents.Add(clone);
        }

        foreach (TExecutionPeriod period in denormalizedIntervals)
        {
            // reference history must be matched
            // if no reference event exists, no event can exist
            if (!HasReferenceEventAt(period.CoalesceFrom))
            {
                // interval must be removed
                // the interval is NOT added to updatedDenormalizedEvents
            }
            else
            {
                TSubEvent? @event = denormalizedEvents.FirstOrDefault(e => e.ExecutionPeriod!.Contains(period.CoalesceFrom));
                if (@event != null)
                {
                    // an event already exists in the current history
                    // should be overwritten, or left alone?
                    if (@event.ExecutionPeriod!.IsCompletelyContainedWithin(newPeriod)
                        && HasPermissionEventAt(period.CoalesceFrom))
                    {
                        CreateNewEventOnInterval(period);
                    }
                    else
                    {
                        // stays the same
                        updatedDenormalizedEvents.Add(@event);
                    }
                }
                else
                {
                    // no event exists in the current history
                    // must a new event be added in this spot?
                    if ((newPeriod.CoalesceFrom.CompareTo(period.CoalesceFrom) <= 0)
                        && (period.CoalesceFrom.CompareTo(newPeriod.CoalesceTo) < 0)
                        && HasPermissionEventAt(period.CoalesceFrom))
                    {
                        CreateNewEventOnInterval(period);
                    }
                }
            }
        }

        // normalize again
        List<TSubEvent> normalizedEvents = NormalizeEvents(updatedDenormalizedEvents);

        // apply changes
        Apply(normalizedEvents);
    }

    /// <inheritdoc />
    public void Create(TPermissionHistory? permissionHistory, TSubEvent @event)
        => ExecuteWithinAnotherPermissionHistory(permissionHistory, () => Create(@event));

    /// <inheritdoc />
    public void Delete(TExecutionPeriod? executionPeriod)
    {
        if ((executionPeriod == null) || !executionPeriod.IsCivilized)
        {
            throw new InternalProgrammingError("Delete can only be called with a civilized period!");
        }

        // helper variables
        TExecution? from = executionPeriod.From;
        TExecution? to = executionPeriod.To;

        // denormalized intervals
        List<TExecutionPeriod> denormalizedIntervals = CalculateDenormalizedIntervals(from, to);

        // denormalized events, semantically the same as actual-events
        List<TSubEvent> denormalizedEvents = BuildDenormalizedEvents(denormalizedIntervals);

        // operation events, denormalized

        // execute change
        TExecutionPeriod removedPeriod = executionPeriod;
        List<TSubEvent> updatedDenormalizedEvents = new ();

        foreach (TExecutionPeriod period in denormalizedIntervals)
        {
            // reference history must be matched
            // if no reference event exists, no event can exist
            if (!HasReferenceEventAt(period.CoalesceFrom))
            {
                // interval must be removed
                // the interval is NOT added to updatedDenormalizedEvents
            }
            else
            {
                TSubEvent? @event = denormalizedEvents.FirstOrDefault(e => e.ExecutionPeriod!.Contains(period.CoalesceFrom));
                if (@event != null)
                {
                    // an event already exists in the current history
                    // should be overwritten, or left alone?
                    if (@event.ExecutionPeriod!.IsCompletelyContainedWithin(removedPeriod)
                        && HasPermissionEventAt(period.CoalesceFrom))
                    {
                        // interval must be removed
                        // interval is NOT added to updatedDenormalizedEvents
                    }
                    else
                    {
                        // stays the same
                        updatedDenormalizedEvents.Add(@event);
                    }
                }
            }
        }

        // normalize again
        List<TSubEvent> normalizedEvents = NormalizeEvents(updatedDenormalizedEvents);

        // apply changes
        Apply(normalizedEvents);
    }

    /// <inheritdoc />
    public void Delete(TPermissionHistory? permissionHistory, TExecutionPeriod executionPeriod)
        => ExecuteWithinAnotherPermissionHistory(permissionHistory, () => Delete(executionPeriod));

    /// <inheritdoc />
    public void Update(TSubEvent historyEvent, TExecutionPeriod newExecutionPeriod, bool sticky)
    {
        if (EventStore.IsTransient(historyEvent, HistoryEventStoreContext))
        {
            throw new InternalProgrammingError("Update can only be called on a non-transient existing event!");
        }

        if (!ActualEvents.Contains(historyEvent))
        {
            throw new ObjectAlreadyChangedException(
                historyEvent.GetType().Name,
                historyEvent.Id!,
                "Update can only be called on an actual event in the history!");
        }

        if ((newExecutionPeriod == null) || !newExecutionPeriod.IsCivilized)
        {
            throw new InternalProgrammingError("Update can only be called with a civilized execution period!");
        }

        TSubEvent cloneOriginal = CloneEvent(historyEvent);
        cloneOriginal.ExecutionPeriod = new TExecutionPeriod { From = newExecutionPeriod.From, To = newExecutionPeriod.To };
        Update(historyEvent, cloneOriginal, sticky);
    }

    /// <inheritdoc />
    public void Update(
        TPermissionHistory? permissionHistory,
        TSubEvent @event,
        TExecutionPeriod newExecutionPeriod,
        bool sticky)
        => ExecuteWithinAnotherPermissionHistory(permissionHistory, () => Update(@event, newExecutionPeriod, sticky));

    public void Update(TSubEvent historyEvent, TSubEvent newHistoryEvent, bool sticky)
    {
        if (EventStore.IsTransient(historyEvent, HistoryEventStoreContext))
        {
            throw new InternalProgrammingError("Update can only be called on a non-transient existing event!");
        }

        if (!ActualEvents.Contains(historyEvent))
        {
            throw new InternalProgrammingError("Update can only be called on an actual event in the history!");
        }

        if (!EventStore.IsTransient(newHistoryEvent, HistoryEventStoreContext))
        {
            throw new InternalProgrammingError("Update can only be called with a transient replacement event!");
        }

        // add constraints on the new execution period
        // in short: there must always be an overlap between the old and the new execution period
        // this makes the code here a little bit easier because there are fewer cases to cover
        // it ensures we get one of the following possibilities for the update
        // - original [X,Y[
        // - new      [X',Y'[
        // one of the following possibilities
        // - X'...X............Y....Y'
        // - .....X...X'.......Y....Y'
        // - X'...X........Y'..Y......
        // - .....X...X'...Y'..Y......
        // that means there will always be 3 intervals to deal with: pre, middle and post
        PPWCode.Vernacular.Contracts.I.Contract.Assert(historyEvent.ExecutionPeriod != null);
        PPWCode.Vernacular.Contracts.I.Contract.Assert(newHistoryEvent.ExecutionPeriod != null);
        if (!historyEvent.ExecutionPeriod.Overlaps(newHistoryEvent.ExecutionPeriod))
        {
            throw CreateInvalidExecutionPeriodUpdate();
        }

        // prepare some stuffs
        TSubEvent? clonePreviousAdjacent = null;
        TSubEvent? cloneNextAdjacent = null;
        if (sticky)
        {
            LinkedList<TSubEvent> linkedActualEvents = new (ActualEvents);
            LinkedListNode<TSubEvent>? current = linkedActualEvents.First;
            while (current != null)
            {
                if (current.Value == historyEvent)
                {
                    // adjacent previous ?
                    if ((current.Previous != null)
                        && current.Previous.Value.ExecutionPeriod!.CoalesceTo.Equals(current.Value.ExecutionPeriod!.CoalesceFrom))
                    {
                        clonePreviousAdjacent = CloneEvent(current.Previous.Value);
                    }

                    // adjacent next ?
                    if ((current.Next != null)
                        && current.Next.Value.ExecutionPeriod!.CoalesceFrom.Equals(current.Value.ExecutionPeriod!.CoalesceTo))
                    {
                        cloneNextAdjacent = CloneEvent(current.Next.Value);
                    }

                    break;
                }

                current = current.Next;
            }
        }

        // pre-interval
        if (historyEvent.ExecutionPeriod.CoalesceFrom.CompareTo(newHistoryEvent.ExecutionPeriod!.CoalesceFrom) < 0)
        {
            TExecutionPeriod newExecutionPeriod =
                new ()
                {
                    From = historyEvent.ExecutionPeriod.From,
                    To = newHistoryEvent.ExecutionPeriod.From
                };

            // sticky and adjacent available, then fill that area, else delete
            if (sticky && (clonePreviousAdjacent != null))
            {
                clonePreviousAdjacent.ExecutionPeriod = newExecutionPeriod;
                Create(clonePreviousAdjacent);
            }
            else
            {
                Delete(newExecutionPeriod);
            }
        }

        // middle-interval, always newFrom ⇒ newTo
        Create(newHistoryEvent);

        // post-interval
        if (newHistoryEvent.ExecutionPeriod.CoalesceTo.CompareTo(historyEvent.ExecutionPeriod.CoalesceTo) < 0)
        {
            TExecutionPeriod newExecutionPeriod =
                new ()
                {
                    From = newHistoryEvent.ExecutionPeriod.To,
                    To = historyEvent.ExecutionPeriod.To
                };

            // sticky and adjacent available, then fill that area, else delete
            if (sticky && (cloneNextAdjacent != null))
            {
                cloneNextAdjacent.ExecutionPeriod = newExecutionPeriod;
                Create(cloneNextAdjacent);
            }
            else
            {
                Delete(newExecutionPeriod);
            }
        }
    }

    /// <inheritdoc />
    public void Update(
        TPermissionHistory? permissionHistory,
        TSubEvent @event,
        TSubEvent newEvent,
        bool sticky)
        => ExecuteWithinAnotherPermissionHistory(permissionHistory, () => Update(@event, newEvent, sticky));

    /// <inheritdoc />
    public Task<ISet<TSubEvent>> ProcessAsync(TKnowledge transactionTime, Func<TSubEvent, THistoryEventStoreContext?, CancellationToken, Task>? onCreate = null, CancellationToken cancellationToken = default)
        => InternalProcessAsync(can => EventStore.ProcessAsync(transactionTime, HistoryEventStoreContext, onCreate, can), cancellationToken);

    /// <inheritdoc />
    public Task<ISet<TSubEvent>> ProcessAsync(Func<TSubEvent, THistoryEventStoreContext?, CancellationToken, Task>? onCreate = null, CancellationToken cancellationToken = default)
        => InternalProcessAsync(can => EventStore.ProcessAsync(HistoryEventStoreContext, onCreate, can), cancellationToken);

    private async Task<ISet<TSubEvent>> InternalProcessAsync(Func<CancellationToken, Task<ISet<TSubEvent>>> eventStoreProcess, CancellationToken cancellationToken = default)
    {
        ISet<TSubEvent> result = await eventStoreProcess(cancellationToken).ConfigureAwait(false);
        foreach (TSubEvent transient in
                 Events
                     .Where(e => EventStore.IsTransient(e, HistoryEventStoreContext))
                     .ToList())
        {
            Events.Remove(transient);
        }

        return result;
    }

    /// <summary>
    ///     Checks whether a permission event is available at the given <paramref name="date" />.
    /// </summary>
    /// <param name="date">the given date</param>
    /// <returns>
    ///     <c>true</c> if a permission event exists at the given <paramref name="date" />;
    ///     <c>false</c> otherwise
    /// </returns>
    private bool HasPermissionEventAt(TExecution date)
        => (PermissionHistory == null) || PermissionHistory.HasPeriodAt(date);

    /// <summary>
    ///     Checks whether a reference event is available at the given <paramref name="date" />.
    /// </summary>
    /// <param name="date">the given date</param>
    /// <returns>
    ///     <c>true</c> if a reference event exists at the given <paramref name="date" />;
    ///     <c>false</c> otherwise
    /// </returns>
    private bool HasReferenceEventAt(TExecution date)
        => (ReferenceHistory == null) || ReferenceHistory.HasPeriodAt(date);

    protected abstract Exception CreateInvalidExecutionPeriodUpdate();

    /// <summary>
    ///     Creates a clone of the given event.
    /// </summary>
    /// <param name="event">the original event</param>
    /// <returns>
    ///     A new event cloned from the <paramref name="event" />.
    ///     If the method <see cref="IHistoryEvent{TKnowledgePeriod,TKnowledge,TOwner,TEvent}.HasIdenticalEventProperties" />
    ///     and their execution periods are also equal, is called on the original event (<paramref name="event" />)
    ///     with the clone as parameter, this method will return true.
    ///     The <see cref="IKnowledgePeriod{TKnowledgePeriod,TKnowledge}.KnowledgePeriod" /> is initialized
    ///     with <see cref="TransactionTime" /> for the <typeparamref name="TExecutionPeriod"/>.From and an empty <typeparamref name="TExecutionPeriod"/>.To />.
    /// </returns>
    /// <remarks>
    ///     This method should be overridden in any subclass of <typeparamref name="TSubEvent"/>.
    /// </remarks>
    protected TSubEvent CloneEvent(TSubEvent @event)
    {
        TSubEvent clone =
            new ()
            {
                KnowledgePeriod = new TKnowledgePeriod { From = TransactionTime, To = null }
            };
        CopyTo(@event, clone);
        return clone;
    }

    /// <summary>
    ///     This is a helper method that should be implemented on each non-abstract subclass.
    /// </summary>
    /// <remarks>
    ///     The actual code that should be used in the concrete subclass is this:
    ///     <code>source.CloneTo(dest);</code>
    /// </remarks>
    /// <param name="source">original</param>
    /// <param name="dest">clone</param>
    protected abstract void CopyTo(TSubEvent source, TSubEvent dest);

    /// <summary>
    ///     This method scans all relevant dates in the existing periods coming from
    ///     <see cref="ReferenceHistory" />, <see cref="PermissionHistory" /> and <see cref="ActualEvents" />.
    ///     Also, the given dates (<paramref name="from" /> and <paramref name="to" />) are taken into account.
    ///     Based on these dates a timeline is constructed consisting of consecutive, adjacent <typeparamref name="TExecutionPeriod"/>s.
    /// </summary>
    /// <param name="from">extra from date</param>
    /// <param name="to">extra to date</param>
    /// <returns>A timeline of consecutive, adjacent intervals</returns>
    private List<TExecutionPeriod> CalculateDenormalizedIntervals(TExecution? from, TExecution? to)
    {
        // calculate different dates
        TExecutionPeriod executionPeriod = new () { From = from, To = to };
        IEnumerable<TExecution> dates = [executionPeriod.CoalesceFrom, executionPeriod.CoalesceTo];
        dates = dates.Concat(ActualPeriods.SelectMany(p => new[] { p.CoalesceFrom, p.CoalesceTo }));

        // add reference history
        if (ReferenceHistory != null)
        {
            dates = dates.Concat(ReferenceHistory.Periods.SelectMany(p => new[] { p.CoalesceFrom, p.CoalesceTo }));

            // for reference history, also needs to add minus infinity and positive infinity
            // this needs to be done to ensure erasure for periods that do not have a match in the reference history
            dates = dates.Concat([_infinitiveExecutionPeriod.CoalesceFrom, _infinitiveExecutionPeriod.CoalesceTo]);
        }

        // add permissions history
        if (PermissionHistory != null)
        {
            dates = dates.Concat(PermissionHistory.Periods.SelectMany(p => new[] { p.CoalesceFrom, p.CoalesceTo }));
        }

        dates = dates.Distinct().OrderBy(dt => dt);

        // create intervals
        List<TExecutionPeriod> intervals = new ();
        dates.Aggregate(
            (x, y) =>
            {
                TExecution? xx = !x.Equals(_infinitiveExecutionPeriod.CoalesceFrom) ? x : null;
                TExecution? yy = !y.Equals(_infinitiveExecutionPeriod.CoalesceTo) ? y : null;
                intervals.Add(new TExecutionPeriod { From = xx, To = yy });
                return y;
            });

        return intervals;
    }

    /// <summary>
    ///     Creates a list of events, semantically the same to the current <see cref="ActualEvents" />,
    ///     but in intervals corresponding to the given <paramref name="denormalizedIntervals" />.
    /// </summary>
    private List<TSubEvent> BuildDenormalizedEvents(IList<TExecutionPeriod> denormalizedIntervals)
    {
        // run over the denormalized intervals
        List<TSubEvent> events = new ();
        foreach (TExecutionPeriod interval in denormalizedIntervals)
        {
            // get original event at date
            TSubEvent? @event = GetActualEventAt(interval.CoalesceFrom);
            if (@event != null)
            {
                // clone the original event
                TSubEvent clone = CloneEvent(@event);
                events.Add(clone);

                // set execution dates
                clone.ExecutionPeriod = new TExecutionPeriod { From = interval.From, To = interval.To };
            }
        }

        return events;
    }

    /// <summary>
    ///     Creates a list of events, semantically the same to the given <paramref name="denormalizedEvents" />
    ///     but optimized to have as little events as possible, by merging consecutive events with
    ///     identical properties.
    /// </summary>
    /// <param name="denormalizedEvents">the given list of events</param>
    /// <returns>An optimized list of events</returns>
    private List<TSubEvent> NormalizeEvents(List<TSubEvent> denormalizedEvents)
    {
        TSubEvent? previousEvent = null;
        List<TSubEvent> events = new ();
        foreach (TSubEvent @event in denormalizedEvents)
        {
            // check if consecutive & identical
            if ((previousEvent != null)
                && previousEvent.ExecutionPeriod!.CoalesceTo.Equals(@event.ExecutionPeriod!.CoalesceFrom)
                && previousEvent.HasIdenticalEventProperties(@event))
            {
                // merge with previous event
                previousEvent.ExecutionPeriod =
                    new TExecutionPeriod
                    {
                        From = previousEvent.ExecutionPeriod.From,
                        To = @event.ExecutionPeriod.To
                    };
            }
            else
            {
                // create a new one
                TSubEvent clone = CloneEvent(@event);
                events.Add(clone);
                previousEvent = clone;
            }
        }

        return events;
    }

    private void Apply(List<TSubEvent> normalizedEvents)
    {
        LinkedList<TSubEvent> originalEvents = new (ActualEvents);
        LinkedList<TSubEvent> tobeEvents = new (normalizedEvents);

        LinkedListNode<TSubEvent>? originalNode = originalEvents.First;
        LinkedListNode<TSubEvent>? tobeNode = tobeEvents.First;

        while ((originalNode != null) || (tobeNode != null))
        {
            // originals completed
            if (originalNode == null)
            {
                while (tobeNode != null)
                {
                    // tobe exists, but no old original ⇒ all new creates
                    EventStore.Open(tobeNode.Value, TransactionTime, HistoryEventStoreContext);
                    Events.Add(tobeNode.Value);
                    tobeNode = tobeNode.Next;
                }

                continue;
            }

            // tobe's completed
            if (tobeNode == null)
            {
                while (originalNode != null)
                {
                    // tobe does not exist, but old exists ⇒ close all events
                    EventStore.Close(originalNode.Value, TransactionTime, HistoryEventStoreContext);
                    originalNode = originalNode.Next;
                }

                continue;
            }

            // originals behind, catch up
            if (originalNode.Value.ExecutionPeriod!.CoalesceFrom.CompareTo(tobeNode.Value.ExecutionPeriod!.CoalesceFrom) < 0)
            {
                EventStore.Close(originalNode.Value, TransactionTime, HistoryEventStoreContext);
                originalNode = originalNode.Next;

                continue;
            }

            // originals at the same level as tobe's
            bool identical =
                originalNode.Value.HasIdenticalEventProperties(tobeNode.Value)
                && originalNode.Value.ExecutionPeriod.Equals(tobeNode.Value.ExecutionPeriod);
            if (identical)
            {
                originalNode = originalNode.Next;
                tobeNode = tobeNode.Next;

                continue;
            }

            // original not before tobe, tobe takes precedence
            EventStore.Open(tobeNode.Value, TransactionTime, HistoryEventStoreContext);
            Events.Add(tobeNode.Value);
            tobeNode = tobeNode.Next;
        }
    }

    private void ExecuteWithinAnotherPermissionHistory(TPermissionHistory? permissionHistory, Action action)
    {
        _permissionHistoryStack.Push(_permissionHistory);
        try
        {
            _permissionHistory = permissionHistory;
            action.Invoke();
        }
        finally
        {
            _permissionHistory = _permissionHistoryStack.Pop();
        }
    }
}
