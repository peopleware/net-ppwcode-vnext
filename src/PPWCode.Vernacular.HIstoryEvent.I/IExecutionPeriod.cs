using PPWCode.Util.Time.I;

namespace PPWCode.Vernacular.HistoryEvent.I;

public interface IExecutionPeriod<T>
    where T : struct, IComparable<T>, IEquatable<T>
{
    IPeriod<T> ExecutionPeriod { get; }
}
