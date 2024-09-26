namespace PPWCode.Vernacular.HistoryEvent.I;

public interface IHistoryEventContext<out T>
    where T : struct, IComparable<T>, IEquatable<T>
{
}
