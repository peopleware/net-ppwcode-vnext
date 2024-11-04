using System.Text.RegularExpressions;

namespace PPWCode.Util.Time.I.Tests;

public class DateOnlyPeriodMultiHistoryTests : PeriodMultiHistoryTests<DateOnlyPeriod, DateOnly>
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
    protected override Regex PeriodRegex
        => new (@"^\[\s*(?<point1>\d{4}-\d{1,2}-\d{1,2}|null)\s*,\s*(?<point2>\d{4}-\d{1,2}-\d{1,2}|null)\s*\[$", RegexOptions.Compiled);

    /// <inheritdoc />
    protected override DateOnlyPeriod CreatePeriod(DateOnly? from, DateOnly? to)
        => new (from, to);

    /// <inheritdoc />
    protected override PeriodMultiHistory<DateOnlyPeriod, DateOnly> CreateMultiPeriodHistory(IEnumerable<DateOnlyPeriod> periods)
        => new DateOnlyPeriodMultiHistory(periods);
}
