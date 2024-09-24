namespace PPWCode.Util.Time.I;

public class PeriodComparer<T> : IEqualityComparer<IPeriod<T>>
    where T : struct, IComparable<T>, IEquatable<T>
{
    public bool Equals(IPeriod<T>? x, IPeriod<T>? y)
    {
        if (ReferenceEquals(x, y))
        {
            return true;
        }

        if (x is null || y is null)
        {
            return false;
        }

        return x.From.Equals(y.From) && x.To.Equals(y.To);
    }

    /// <inheritdoc />
    public int GetHashCode(IPeriod<T> obj)
    {
        unchecked
        {
            return (obj.From.GetHashCode() * 397) ^ obj.To.GetHashCode();
        }
    }
}
