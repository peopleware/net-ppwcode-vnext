using PPWCode.Util.Time.I;

namespace PPWCode.Vernacular.RequestContext.I;

public class RequestContext<T> : IRequestContext<T>
    where T : struct, IComparable<T>, IEquatable<T>
{
    public RequestContext(ITimeProvider<T> timeProvider, IReadOnlyProvider readOnlyProvider)
    {
        IsReadOnly = readOnlyProvider.IsReadOnly;
        RequestTimestamp = timeProvider.Now;
    }

    /// <inheritdoc />
    public bool IsReadOnly { get; }

    /// <inheritdoc />
    public T RequestTimestamp { get; }
}
