namespace PPWCode.Util.Time.I.Tests;

public class DateOnlyPeriodMultiHistory : PeriodMultiHistory<DateOnlyPeriod, DateOnly>
{
    public DateOnlyPeriodMultiHistory(IEnumerable<DateOnlyPeriod> periods)
        : base(periods)
    {
    }

    protected override DateOnlyPeriod Create(DateOnly? from, DateOnly? to)
        => new (from, to);
}
