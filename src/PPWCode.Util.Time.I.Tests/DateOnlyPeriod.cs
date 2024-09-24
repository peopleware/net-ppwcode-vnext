using PPWCode.Vernacular.Exceptions.V;

namespace PPWCode.Util.Time.I.Tests;

public class DateOnlyPeriod : I.DateOnlyPeriod
{
    public DateOnlyPeriod(DateOnly? from, DateOnly? to)
        : base(from, to)
    {
    }

    /// <inheritdoc />
    protected override SemanticException CreateInvalidExceptionFor(DateOnly? from, DateOnly? to)
        => new ("ERROR_PERIOD_FROM_MUST_BE_STRICTLY_BEFORE_TO");

    /// <inheritdoc />
    protected override IPeriod<DateOnly> Create(DateOnly? from, DateOnly? to)
        => new DateOnlyPeriod(from, to);
}
