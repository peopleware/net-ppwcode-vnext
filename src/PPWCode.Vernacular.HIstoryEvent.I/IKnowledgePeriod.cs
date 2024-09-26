using PPWCode.Util.Time.I;

namespace PPWCode.Vernacular.HistoryEvent.I;

public interface IKnowledgePeriod<T>
    where T : struct, IComparable<T>, IEquatable<T>
{
    IPeriod<T> KnowledgePeriod { get; }
}
