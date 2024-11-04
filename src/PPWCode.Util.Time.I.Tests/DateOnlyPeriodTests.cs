using System.Text.RegularExpressions;

using NUnit.Framework;

namespace PPWCode.Util.Time.I.Tests;

public class DateOnlyPeriodTests : PeriodTests<DateOnlyPeriod, DateOnly>
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
    protected override Regex PeriodRegex
        => new (@"^\[\s*(?<point1>\d{4}-\d{1,2}-\d{1,2}|null)\s*,\s*(?<point2>\d{4}-\d{1,2}-\d{1,2}|null)\s*\[$", RegexOptions.Compiled);

    [TestCase("[null,null[", "2025-01-01", ExpectedResult = true)]
    [TestCase("[2024-09-10,null[", "2025-01-01", ExpectedResult = true)]
    [TestCase("[2024-09-10,2025-01-01[", "2025-01-01", ExpectedResult = false)]
    [TestCase("[2024-09-10,null[", "2024-01-01", ExpectedResult = false)]
    public override bool check_contains_point_in_time(string periodAsString, string d)
        => base.check_contains_point_in_time(periodAsString, d);

    [TestCase("[2025-01-01,2024-09-10[", "2025-01-01")]
    [TestCase("[2025-01-01,2024-09-10[", "2024-06-10")]
    public override void check_non_civilized_contains_point_in_time_throws(string periodAsString, string d)
        => base.check_non_civilized_contains_point_in_time_throws(periodAsString, d);

    [TestCase("[null,null[", "[2024-09-10,2025-01-01[", ExpectedResult = true)]
    [TestCase("[2024-09-10,null[", "[2024-09-10,2025-01-01[", ExpectedResult = true)]
    [TestCase("[2024-09-10,null[", "[2024-09-10,null[", ExpectedResult = true)]
    [TestCase("[2024-09-10,2025-01-01[", "[2024-09-10,2025-01-01[", ExpectedResult = true)]
    [TestCase("[2024-09-10,null[", "[2024-09-09,2025-01-01[", ExpectedResult = false)]
    [TestCase("[2024-09-10,null[", "[2024-09-09,null[", ExpectedResult = false)]
    [TestCase("[2024-09-10,null[", "[2021-09-09,2022-10-01[", ExpectedResult = false)]
    public override bool check_contains_period(string periodAsString1, string periodAsString2)
        => base.check_contains_period(periodAsString1, periodAsString2);

    [TestCase("[2024-09-10,2025-01-01[", "[2025-01-01,2024-09-10[")]
    [TestCase("[null,2025-01-01[", "[2025-01-01,2024-09-10[")]
    [TestCase("[2024-09-10,null[", "[2025-01-01,2024-09-10[")]
    [TestCase("[null,null[", "[2025-01-01,2024-09-10[")]
    [TestCase("[2025-01-01,2024-09-10[", "[2024-09-10,2025-01-01[")]
    [TestCase("[2025-01-01,2024-09-10[", "[2024-09-10,null[")]
    [TestCase("[2025-01-01,2024-09-10[", "[null,2025-01-01[")]
    public override void check_non_civilized_contains_period_throws(string periodAsString1, string periodAsString2)
        => base.check_non_civilized_contains_period_throws(periodAsString1, periodAsString2);

    [TestCase("[null,null[", "[2024-09-10,2025-01-01[", ExpectedResult = true)]
    [TestCase("[2024-09-10,null[", "[2024-09-10,2025-01-01[", ExpectedResult = true)]
    [TestCase("[2024-09-10,null[", "[2024-09-10,null[", ExpectedResult = true)]
    [TestCase("[2024-09-10,2025-01-01[", "[2024-09-10,2025-01-01[", ExpectedResult = true)]
    [TestCase("[2024-09-10,null[", "[2024-09-09,2025-01-01[", ExpectedResult = true)]
    [TestCase("[2024-09-10,null[", "[2024-09-09,null[", ExpectedResult = true)]
    [TestCase("[2024-09-10,null[", "[2021-09-09,2022-10-01[", ExpectedResult = false)]
    public override bool check_overlaps(string periodAsString1, string periodAsString2)
        => base.check_overlaps(periodAsString1, periodAsString2);

    [TestCase("[2024-09-10,2025-01-01[", "[2025-01-01,2024-09-10[")]
    [TestCase("[null,2025-01-01[", "[2025-01-01,2024-09-10[")]
    [TestCase("[2024-09-10,null[", "[2025-01-01,2024-09-10[")]
    [TestCase("[null,null[", "[2025-01-01,2024-09-10[")]
    [TestCase("[2025-01-01,2024-09-10[", "[2024-09-10,2025-01-01[")]
    [TestCase("[2025-01-01,2024-09-10[", "[2024-09-10,null[")]
    [TestCase("[2025-01-01,2024-09-10[", "[null,2025-01-01[")]
    public override void check_non_civilized_overlaps_throws(string periodAsString1, string periodAsString2)
        => base.check_non_civilized_overlaps_throws(periodAsString1, periodAsString2);

    [TestCase("[null,null[", "[null,null[", ExpectedResult = "[null,null[")]
    [TestCase("[null,null[", "[2025-01-01,null[", ExpectedResult = "[2025-01-01,null[")]
    [TestCase("[2024-09-10,null[", "[2025-01-01,null[", ExpectedResult = "[2025-01-01,null[")]
    [TestCase("[2024-09-10,2025-02-01[", "[2025-01-01,null[", ExpectedResult = "[2025-01-01,2025-02-01[")]
    public override string check_overlapping_period(string periodAsString1, string periodAsString2)
        => base.check_overlapping_period(periodAsString1, periodAsString2);

    [TestCase("[2024-09-10,2025-01-01[", "[2025-01-01,2024-09-10[")]
    [TestCase("[null,2025-01-01[", "[2025-01-01,2024-09-10[")]
    [TestCase("[2024-09-10,null[", "[2025-01-01,2024-09-10[")]
    [TestCase("[null,null[", "[2025-01-01,2024-09-10[")]
    [TestCase("[2025-01-01,2024-09-10[", "[2024-09-10,2025-01-01[")]
    [TestCase("[2025-01-01,2024-09-10[", "[2024-09-10,null[")]
    [TestCase("[2025-01-01,2024-09-10[", "[null,2025-01-01[")]
    public override void check_non_civilized_overlapping_period_throws(string periodAsString1, string periodAsString2)
        => base.check_non_civilized_overlapping_period_throws(periodAsString1, periodAsString2);

    [TestCase("[null,null[", "[null,null[", ExpectedResult = true)]
    [TestCase("[2025-01-01,null[", "[null,null[", ExpectedResult = true)]
    [TestCase("[2025-01-01,2025-02-01[", "[null,null[", ExpectedResult = true)]
    [TestCase("[2025-01-01,2025-02-01[", "[2025-01-01,2025-02-01[", ExpectedResult = true)]
    [TestCase("[2025-01-01,2025-02-01[", "[2024-01-01,2025-02-01[", ExpectedResult = true)]
    [TestCase("[2025-01-01,2025-02-01[", "[2025-01-01,2026-02-01[", ExpectedResult = true)]
    [TestCase("[2025-01-01,2025-02-01[", "[2024-01-01,2026-02-01[", ExpectedResult = true)]
    [TestCase("[null,null[", "[2025-01-01,null[", ExpectedResult = false)]
    public override bool check_is_completely_contained_within(string periodAsString1, string periodAsString2)
        => base.check_is_completely_contained_within(periodAsString1, periodAsString2);

    [TestCase("[2024-09-10,2025-01-01[", "[2025-01-01,2024-09-10[")]
    [TestCase("[null,2025-01-01[", "[2025-01-01,2024-09-10[")]
    [TestCase("[2024-09-10,null[", "[2025-01-01,2024-09-10[")]
    [TestCase("[null,null[", "[2025-01-01,2024-09-10[")]
    [TestCase("[2025-01-01,2024-09-10[", "[2024-09-10,2025-01-01[")]
    [TestCase("[2025-01-01,2024-09-10[", "[2024-09-10,null[")]
    [TestCase("[2025-01-01,2024-09-10[", "[null,2025-01-01[")]
    public override void check_non_civilized_is_completely_contained_within_throws(string periodAsString1, string periodAsString2)
        => base.check_non_civilized_is_completely_contained_within_throws(periodAsString1, periodAsString2);
}
