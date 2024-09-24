namespace PPWCode.Util.Time.I.Tests;

public class DateOnlyPeriodHistoryTests : PeriodHistoryTests<DateOnlyPeriod, DateOnly>
{
    /// <inheritdoc />
    protected override DateOnly AddMonths(DateOnly date, int i)
        => date.AddMonths(i);

    /// <inheritdoc />
    protected override DateOnly? ConvertFromString(string? value)
        => string.IsNullOrWhiteSpace(value) ? null : DateOnly.Parse(value);

    /// <inheritdoc />
    protected override string ConvertToString(DateOnly? value)
        => value is null ? "null" : $"{value.Value:yyyy-MM-dd}";

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
