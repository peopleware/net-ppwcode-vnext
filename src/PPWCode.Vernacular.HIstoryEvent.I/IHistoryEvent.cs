namespace PPWCode.Vernacular.HistoryEvent.I;

public interface IHistoryEvent<T, TOwner> : IKnowledgePeriod<T>
    where T : struct, IComparable<T>, IEquatable<T>
{
    TOwner Owner { get; }

    bool HasIdenticalEventProperties(IHistoryEvent<T, TOwner> other);
    bool HasIdenticalExecutionPeriod(IHistoryEvent<T, TOwner> other);
    void SetKnowledgePeriod(T? from, T? to);
    void SetExecutionPeriod(T? from, T? to);
}
