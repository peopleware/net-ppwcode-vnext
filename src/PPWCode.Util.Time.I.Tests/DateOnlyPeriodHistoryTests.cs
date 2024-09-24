namespace PPWCode.Util.Time.I.Tests;

public class DateOnlyPeriodHistoryTests : PeriodHistoryTests<DateOnlyPeriod, DateOnly>
{
    /// <inheritdoc />
    protected override DateOnly AddMonths(DateOnly date, int i)
        => date.AddMonths(i);

    /// <inheritdoc />
    protected override DateOnlyPeriod CreatePeriod(DateOnly? from, DateOnly? to)
        => new (from, to);

    /// <inheritdoc />
    protected override PeriodHistory<DateOnlyPeriod, DateOnly> CreatePeriodHistory(IEnumerable<DateOnlyPeriod> periods)
        => new DateOnlyPeriodHistory(periods);

    /// <inheritdoc />
    protected override DateOnly Create(int year, int month, int day)
        => new (year, month, day);
}
