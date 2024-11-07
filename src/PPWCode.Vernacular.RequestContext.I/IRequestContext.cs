namespace PPWCode.Vernacular.RequestContext.I;

public interface IRequestContext<out T>
    where T : struct, IComparable<T>, IEquatable<T>
{
    bool IsReadOnly { get; }
    T RequestTimestamp { get; }
    string IdentityName { get; }
}
