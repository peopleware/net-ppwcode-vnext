using PPWCode.Vernacular.Exceptions.V;

namespace PPWCode.Util.Time.I.Tests;

public class PeriodTest : DateOnlyPeriod
{
    public PeriodTest(DateOnly? from, DateOnly? to)
        : base(from, to)
    {
    }

    /// <inheritdoc />
    protected override SemanticException CreateInvalidExceptionFor(DateOnly? from, DateOnly? to)
        => new ("ERROR_PERIOD_FROM_MUST_BE_STRICTLY_BEFORE_TO");

    /// <inheritdoc />
    protected override IPeriod<DateOnly> Create(DateOnly? from, DateOnly? to)
        => new PeriodTest(from, to);
}
