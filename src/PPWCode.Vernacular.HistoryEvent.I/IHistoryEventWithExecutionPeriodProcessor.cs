﻿// Copyright 2025 by PeopleWare n.v..
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
using PPWCode.Vernacular.Persistence.V;

namespace PPWCode.Vernacular.HistoryEvent.I
{
    /// <summary>
    ///     This interface represents the complete history of
    ///     a certain type of event.
    /// </summary>
    /// <typeparam name="TOwner">generic type of the owner</typeparam>
    /// <typeparam name="TSubEvent">generic type of the event</typeparam>
    /// <typeparam name="TId">generic idenyification type of TEvent</typeparam>
    /// <typeparam name="TKnowledgePeriod">generic type of the knowledge period</typeparam>
    /// <typeparam name="TKnowledge">generic type of the knowledge period members</typeparam>
    /// <typeparam name="TExecutionPeriod">generic type of the execution period</typeparam>
    /// <typeparam name="TExecution">generic type of the execution period members</typeparam>
    /// <typeparam name="TEvent">type of the implementation</typeparam>
    /// <typeparam name="THistoryEventStoreContext">type of the optional eventProcessorContext</typeparam>
    /// <typeparam name="TReferenceHistory">type of the reference history in the processor context</typeparam>
    /// <typeparam name="TPermissionHistory">type of the permission history in the processor context</typeparam>
    public interface IHistoryEventWithExecutionPeriodProcessor<TOwner, TSubEvent, TId, TKnowledgePeriod, TKnowledge, TExecutionPeriod, TExecution, TEvent, THistoryEventStoreContext, TReferenceHistory, TPermissionHistory>
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
        /// <summary>
        ///      Original eventProcessorContext that we need to process the history.
        /// </summary>
        HistoryEventProcessorContext<TOwner, TSubEvent, TId, TKnowledgePeriod, TKnowledge, TExecutionPeriod, TExecution, TEvent, THistoryEventStoreContext, TReferenceHistory, TPermissionHistory> EventProcessorContext { get; }

        /// <summary>
        ///     The <see cref="ReferenceHistory" /> is an ordered set of events that must be used as a reference timeline.
        ///     For every point in time that has an event in the reference timeline, this history must also have an event
        ///     on that point, and vice versa. This is an invariant that must be respected during updates on this history.
        /// </summary>
        TReferenceHistory? ReferenceHistory { get; }

        /// <summary>
        ///     The <see cref="PermissionHistory" /> is an ordered set of events that represents the permissions of the user
        ///     of this history. The user can only change something in this history on a given point in time if the
        ///     PermissionHistory has an event at that point in time.
        /// </summary>
        TPermissionHistory? PermissionHistory { get; }

        /// <summary>
        ///     The reference time point at which all operations on the history take place.
        /// </summary>
        TKnowledge TransactionTime { get; }

        /// <summary>
        ///     The complete list of events of type <typeparamref name="TSubEvent" /> within this history.
        /// </summary>
        IList<TSubEvent> Events { get; }

        /// <summary>
        ///     Returns the history events that are actual:
        ///     these are the events that do not have a knowledge <c>to</c> date.
        ///     The history events are ordered based on the <c>from</c> date of the
        ///     <c>execution period</c>.
        /// </summary>
        IList<TSubEvent> ActualEvents { get; }

        /// <summary>
        ///     This returns a list of execution periods that correspond to the list of the
        ///     currently active events (see <see cref="ActualEvents" />).
        /// </summary>
        IList<TExecutionPeriod> ActualPeriods { get; }

        /// <summary>
        ///     Gets the currently active event at the given <paramref name="date" />.
        /// </summary>
        /// <param name="date">the given date</param>
        /// <returns>
        ///     The event at the given <paramref name="date" />, or <c>null</c> if no such event is found.
        /// </returns>
        TSubEvent? GetActualEventAt(TExecution date);

        /// <summary>
        ///     Creates the new, given, <paramref name="event" /> and adds it in the history.
        ///     During creation, the configured <see cref="PermissionHistory" /> and
        ///     <see cref="ReferenceHistory" /> are respected. After creation the
        ///     <see cref="Events" /> will be updated.
        /// </summary>
        /// <param name="event">the given, transient, event</param>
        void Create(TSubEvent @event);

        /// <summary>
        ///     Creates the new, given, <paramref name="event" /> and adds it in the history.
        ///     During creation, the given <see cref="PermissionHistory" /> and the configured
        ///     <see cref="ReferenceHistory" /> are respected. After creation the
        ///     <see cref="Events" /> will be updated.
        /// </summary>
        /// <param name="permissionHistory">the given permission history</param>
        /// <param name="event">the given, transient, event</param>
        /// <remarks>The given <paramref name="permissionHistory" /> overrules the configured <see cref="PermissionHistory" />.</remarks>
        void Create(TPermissionHistory? permissionHistory, TSubEvent @event);

        /// <summary>
        ///     Clears the history over the given <paramref name="executionPeriod" />. During the removal and update of the
        ///     existing <see cref="Events" /> the configured <see cref="PermissionHistory" /> and <see cref="ReferenceHistory" />
        ///     are respected. After the clearance, the <see cref="Events" /> will be updated.
        /// </summary>
        /// <param name="executionPeriod">the given period</param>
        void Delete(TExecutionPeriod? executionPeriod);

        /// <summary>
        ///     Clears the history over the given <paramref name="executionPeriod" />. During the removal and update of the
        ///     existing <see cref="Events" /> the given <see cref="PermissionHistory" /> and the configured
        ///     <see cref="ReferenceHistory" />
        ///     are respected. After the clearance, the <see cref="Events" /> will be updated.
        /// </summary>
        /// <param name="permissionHistory">the given permission history</param>
        /// <param name="executionPeriod">the given period</param>
        /// <remarks>The given <paramref name="permissionHistory" /> overrules the configured <see cref="PermissionHistory" />.</remarks>
        void Delete(TPermissionHistory? permissionHistory, TExecutionPeriod executionPeriod);

        /// <summary>
        ///     Updates the given <paramref name="event" /> in the history with a new execution period.  During the update, the
        ///     configured <see cref="PermissionHistory" /> and <see cref="ReferenceHistory" /> are respected. After creation
        ///     the <see cref="Events" /> will be updated.
        /// </summary>
        /// <param name="event">the given, transient, event</param>
        /// <param name="newExecutionPeriod">the new execution period</param>
        /// <param name="sticky">whether the bordering events should also move</param>
        void Update(TSubEvent @event, TExecutionPeriod newExecutionPeriod, bool sticky);

        /// <summary>
        ///     Updates the given <paramref name="event" /> in the history with a new execution period.  During the update, the
        ///     given <see cref="PermissionHistory" /> and the configured <see cref="ReferenceHistory" /> are respected. After
        ///     creation
        ///     the <see cref="Events" /> will be updated.
        /// </summary>
        /// <param name="permissionHistory">the given permission history</param>
        /// <param name="event">the given, transient, event</param>
        /// <param name="newExecutionPeriod">the new execution period</param>
        /// <param name="sticky">whether the bordering events should also move</param>
        /// <remarks>The given <paramref name="permissionHistory" /> overrules the configured <see cref="PermissionHistory" />.</remarks>
        void Update(TPermissionHistory? permissionHistory, TSubEvent @event, TExecutionPeriod newExecutionPeriod, bool sticky);

        /// <summary>
        ///     Updates the given <paramref name="event" /> in the history with the given <paramref name="newEvent" />.
        ///     During the update, the configured <see cref="PermissionHistory" /> and <see cref="ReferenceHistory" />
        ///     are respected. After creation the <see cref="Events" /> will be updated.
        /// </summary>
        /// <param name="event">the given, transient, event</param>
        /// <param name="newEvent">the new not transient event</param>
        /// <param name="sticky">whether the bordering events should expand based on the execution period of the new event</param>
        void Update(TSubEvent @event, TSubEvent newEvent, bool sticky);

        /// <summary>
        ///     Updates the given <paramref name="event" /> in the history with the given <paramref name="newEvent" />.
        ///     During the update, the given <see cref="PermissionHistory" /> and the configured <see cref="ReferenceHistory" />
        ///     are respected. After creation the <see cref="Events" /> will be updated.
        /// </summary>
        /// <param name="permissionHistory">the given permission history</param>
        /// <param name="event">the given, transient, event</param>
        /// <param name="newEvent">the new not transient event</param>
        /// <param name="sticky">whether the bordering events should expand based on the execution period of the new event</param>
        /// <remarks>The given <paramref name="permissionHistory" /> overrules the configured <see cref="PermissionHistory" />.</remarks>
        void Update(TPermissionHistory? permissionHistory, TSubEvent @event, TSubEvent newEvent, bool sticky);

        /// <summary>
        ///     Processes all remaining changes in the underlying event store. And removes any events from the history that are
        ///     still transient after the processing.
        /// </summary>
        /// <param name="transactionTime">use this transaction time as a reference, as the 'current' transaction time</param>
        /// <param name="onCreate">Optional lambda that will be invoked and should be responsible to save the event</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>
        ///     This returns a set of all modified events.  The set contains both events that
        ///     received an end time on the knowledge period, and events that are completely new.
        /// </returns>
        /// <remarks>
        ///     It is possible to use the history for creates/updates/deletes but not use it for the management of the event store.
        ///     In that case, the event store can be reused for multiple histories, and the process method can be called directly
        ///     on the event store.  This is actually preferable and in some cases required: if multiple histories must be created
        ///     in the same request, they should all use the same event store and the process method should only be called at the
        ///     very end when all operations are finished.
        /// </remarks>
        /// <remarks>This method is only to be used during the migration!</remarks>
        /// <exception cref="OperationCanceledException">If the <see cref="CancellationToken" /> is canceled.</exception>
        Task<ISet<TSubEvent>> ProcessAsync(TKnowledge transactionTime, Func<TSubEvent, THistoryEventStoreContext?, CancellationToken, Task>? onCreate = null, CancellationToken cancellationToken = default);

        /// <summary>
        ///     Processes all remaining changes in the underlying event store. And removes any events from the history that are
        ///     still transient after the processing.
        /// </summary>
        /// <param name="onCreate">Optional lambda that will be invoked and should be responsible to save the event</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>
        ///     This returns a set of all modified events.  The set contains both events that
        ///     received an end time on the knowledge period, and events that are completely new.
        /// </returns>
        /// <remarks>
        ///     It is possible to use the history for creates/updates/deletes but not use it for the management of the event store.
        ///     In that case, the event store can be reused for multiple histories, and the process method can be called directly
        ///     on the event store.  This is actually preferable and in some cases required: if multiple histories must be created
        ///     in the same request, they should all use the same event store and the process method should only be called at the
        ///     very end when all operations are finished.
        /// </remarks>
        /// <exception cref="OperationCanceledException">If the <see cref="CancellationToken" /> is canceled.</exception>
        Task<ISet<TSubEvent>> ProcessAsync(Func<TSubEvent, THistoryEventStoreContext?, CancellationToken, Task>? onCreate = null, CancellationToken cancellationToken = default);
    }
}
