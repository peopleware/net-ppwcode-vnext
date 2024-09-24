namespace PPWCode.Util.Time.I.Tests;

public class DateOnlyPeriodMultiHistoryTests : PeriodMultiHistoryTests<DateOnlyPeriod, DateOnly>
{
    /// <inheritdoc />
    protected override DateOnlyPeriod CreatePeriod(DateOnly? from, DateOnly? to)
        => new (from, to);

    /// <inheritdoc />
    protected override PeriodMultiHistory<DateOnlyPeriod, DateOnly> CreateMultiPeriodHistory(IEnumerable<DateOnlyPeriod> periods)
        => new DateOnlyPeriodMultiHistory(periods);

    /// <inheritdoc />
    protected override DateOnly? ConvertFromString(string? value)
        => string.IsNullOrWhiteSpace(value) ? null : DateOnly.Parse(value);

    /// <inheritdoc />
    protected override string ConvertToString(DateOnly? value)
        => value is null ? "null" : $"{value.Value:yyyy-MM-dd}";

    /// <inheritdoc />
    protected override DateOnly Create(int year, int month, int day)
        => new (year, month, day);

    /// <inheritdoc />
    protected override DateOnly AddMonths(DateOnly date, int i)
        => date.AddMonths(i);
}
