using PPWCode.Util.Time.I;

namespace PPWCode.Vernacular.HistoryEvent.I;

public interface IKnowledgePeriod<TKnowledgePeriod, TKnowledge>
    where TKnowledgePeriod : IPeriod<TKnowledge>
    where TKnowledge : struct, IComparable<TKnowledge>, IEquatable<TKnowledge>
{
    TKnowledgePeriod? KnowledgePeriod { get; set; }
}
