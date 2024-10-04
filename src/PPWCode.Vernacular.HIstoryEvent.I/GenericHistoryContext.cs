using PPWCode.Util.Time.I;
using PPWCode.Vernacular.Persistence.V;

namespace PPWCode.Vernacular.HistoryEvent.I;

public class GenericHistoryContext<TOwner, TSubEvent, TId, TKnowledgePeriod, TKnowledge, TExecutionPeriod, TExecution, TEvent, THistoryEventStoreContext>
    where TEvent : IHistoryEvent<TKnowledgePeriod, TKnowledge, TOwner, TEvent>, IPersistentObject<TId>
    where TId : IEquatable<TId>
    where TKnowledgePeriod : Period<TKnowledge>, new()
    where TKnowledge : struct, IComparable<TKnowledge>, IEquatable<TKnowledge>
    where TExecutionPeriod : Period<TExecution>, new()
    where TExecution : struct, IComparable<TExecution>, IEquatable<TExecution>
    where TSubEvent : TEvent
    where THistoryEventStoreContext : IHistoryEventStoreContext
{
    public GenericHistoryContext(
        IEnumerable<TSubEvent> events,
        PeriodHistory<TExecutionPeriod, TExecution>? referenceHistory = default,
        PeriodHistory<TExecutionPeriod, TExecution>? permissionHistory = default,
        THistoryEventStoreContext? historyEventStoreContext = default,
        TKnowledge? transactionTime = default)
    {
        Events = events.ToHashSet();
        ReferenceHistory = referenceHistory;
        PermissionHistory = permissionHistory;
        HistoryEventStoreContext = historyEventStoreContext;
        TransactionTime = transactionTime;
    }

    public ISet<TSubEvent> Events { get; }

    public THistoryEventStoreContext? HistoryEventStoreContext { get; }
    public PeriodHistory<TExecutionPeriod, TExecution>? ReferenceHistory { get; }

    public PeriodHistory<TExecutionPeriod, TExecution>? PermissionHistory { get; }

    public TKnowledge? TransactionTime { get; }
}
