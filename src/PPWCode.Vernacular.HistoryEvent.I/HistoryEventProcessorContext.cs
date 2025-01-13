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
