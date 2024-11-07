using PPWCode.Util.Time.I;

namespace PPWCode.Vernacular.RequestContext.I;

public abstract class RequestContext<T> : IRequestContext<T>
    where T : struct, IComparable<T>, IEquatable<T>
{
    private readonly IIdentityProvider _identityProvider;

    protected RequestContext(
        ITimeProvider<T> timeProvider,
        IReadOnlyProvider readOnlyProvider,
        IIdentityProvider identityProvider)
    {
        _identityProvider = identityProvider;
        IsReadOnly = readOnlyProvider.IsReadOnly;
        RequestTimestamp = timeProvider.Now;
    }

    /// <inheritdoc />
    public bool IsReadOnly { get; }

    /// <inheritdoc />
    public T RequestTimestamp { get; }

    /// <inheritdoc />
    public string IdentityName
        => _identityProvider.IdentityName;
}
