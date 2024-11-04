using PPWCode.Vernacular.Exceptions.V;

namespace PPWCode.Util.Time.I.Tests;

public class DateTimeOffsetPeriod : I.DateTimeOffsetPeriod
{
    public DateTimeOffsetPeriod(DateTimeOffset? from, DateTimeOffset? to)
        : base(from, to)
    {
    }

    /// <inheritdoc />
    protected override SemanticException CreateInvalidExceptionFor(DateTimeOffset? from, DateTimeOffset? to)
        => new ("ERROR_PERIOD_FROM_MUST_BE_STRICTLY_BEFORE_TO");

    /// <inheritdoc />
    protected override IPeriod<DateTimeOffset> Create(DateTimeOffset? from, DateTimeOffset? to)
        => new DateTimeOffsetPeriod(from, to);
}
