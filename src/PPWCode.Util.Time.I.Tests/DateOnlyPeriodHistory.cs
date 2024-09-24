namespace PPWCode.Util.Time.I.Tests;

public class DateOnlyPeriodHistory : PeriodHistory<DateOnlyPeriod, DateOnly>
{
    public DateOnlyPeriodHistory(IEnumerable<DateOnlyPeriod> periods)
        : base(periods)
    {
    }

    /// <inheritdoc />
    protected override DateOnlyPeriod Create(DateOnly? from, DateOnly? to)
        => new (from, to);
}
