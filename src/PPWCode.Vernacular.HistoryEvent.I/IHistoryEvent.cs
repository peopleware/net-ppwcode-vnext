using PPWCode.Util.Time.I;

namespace PPWCode.Vernacular.HistoryEvent.I;

public interface IHistoryEvent<TKnowledgePeriod, TKnowledge, out TOwner, in TSelf> : IKnowledgePeriod<TKnowledgePeriod, TKnowledge>
    where TKnowledge : struct, IComparable<TKnowledge>, IEquatable<TKnowledge>
    where TKnowledgePeriod : IPeriod<TKnowledge>
    where TSelf : IHistoryEvent<TKnowledgePeriod, TKnowledge, TOwner, TSelf>
{
    TOwner Owner { get; }

    bool HasIdenticalEventProperties(TSelf other);
}
