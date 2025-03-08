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
using PPWCode.Vernacular.Persistence.V;

namespace PPWCode.Vernacular.HistoryEvent.I;

public class HistoryEventProcessorContext<TOwner, TSubEvent, TId, TKnowledgePeriod, TKnowledge, TExecutionPeriod, TExecution, TEvent, THistoryEventStoreContext, TReferenceHistory, TPermissionHistory>
    where TEvent : IHistoryEvent<TKnowledgePeriod, TKnowledge, TOwner, TEvent>, IPersistentObject<TId>
    where TId : IEquatable<TId>
    where TKnowledgePeriod : Period<TKnowledge>, new()
    where TKnowledge : struct, IComparable<TKnowledge>, IEquatable<TKnowledge>
    where TExecutionPeriod : Period<TExecution>, new()
    where TExecution : struct, IComparable<TExecution>, IEquatable<TExecution>
    where TSubEvent : TEvent
    where THistoryEventStoreContext : IHistoryEventStoreContext
    where TReferenceHistory : PeriodHistory<TExecutionPeriod, TExecution>
    where TPermissionHistory : PeriodHistory<TExecutionPeriod, TExecution>
{
    public HistoryEventProcessorContext(
        IEnumerable<TSubEvent> events,
        THistoryEventStoreContext? historyEventStoreContext = default,
        TReferenceHistory? referenceHistory = null,
        TPermissionHistory? permissionHistory = null,
        TKnowledge? transactionTime = null)
    {
        Events = events.ToHashSet();
        ReferenceHistory = referenceHistory;
        PermissionHistory = permissionHistory;
        HistoryEventStoreContext = historyEventStoreContext;
        TransactionTime = transactionTime;
    }

    public ISet<TSubEvent> Events { get; }

    public THistoryEventStoreContext? HistoryEventStoreContext { get; }
    public TReferenceHistory? ReferenceHistory { get; }

    public TPermissionHistory? PermissionHistory { get; }

    public TKnowledge? TransactionTime { get; }
}
