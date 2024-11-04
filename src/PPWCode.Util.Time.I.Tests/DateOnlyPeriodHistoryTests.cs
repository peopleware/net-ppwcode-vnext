namespace PPWCode.Util.Time.I.Tests;

public class DateOnlyPeriodHistoryTests : PeriodHistoryTests<DateOnlyPeriod, DateOnly>
{
    /// <inheritdoc />
    protected override string PointToString(DateOnly value)
        => $"{value:yyyy-MM-dd}";

    /// <inheritdoc />
    protected override DateOnly StringToPoint(string value)
        => DateOnly.Parse(value);

    /// <inheritdoc />
    protected override DateOnly CreatePoint(int year, int month, int day)
        => new (year, month, day);

    /// <inheritdoc />
    protected override DateOnly AddToPoint(DateOnly date, int i)
        => date.AddMonths(i);

    /// <inheritdoc />
    protected override DateOnlyPeriod CreatePeriod(DateOnly? from, DateOnly? to)
        => new (from, to);

    /// <inheritdoc />
    protected override PeriodHistory<DateOnlyPeriod, DateOnly> CreatePeriodHistory(IEnumerable<DateOnlyPeriod> periods)
        => new DateOnlyPeriodHistory(periods);
}
