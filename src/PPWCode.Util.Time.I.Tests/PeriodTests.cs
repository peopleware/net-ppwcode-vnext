using NUnit.Framework;

using PPWCode.Vernacular.Exceptions.V;

namespace PPWCode.Util.Time.I.Tests;

[TestFixture]
public abstract class PeriodTests<TPeriod, T> : BasePeriodTests<TPeriod, T>
    where TPeriod : class, IPeriod<T>
    where T : struct, IComparable<T>, IEquatable<T>
{
    [TestCase("[null,null[", "2025-01-01", ExpectedResult = true)]
    [TestCase("[2024-09-10,null[", "2025-01-01", ExpectedResult = true)]
    [TestCase("[2024-09-10,2025-01-01[", "2025-01-01", ExpectedResult = false)]
    [TestCase("[2024-09-10,null[", "2024-01-01", ExpectedResult = false)]
    public bool check_contains_point_in_time(string periodAsString, string d)
    {
        IPeriod<T> period = ConvertStringToPeriod(periodAsString);
        T pointInTime = StringToPoint(d);
        return period.Contains(pointInTime);
    }

    [TestCase("[2025-01-01,2024-09-10[", "2025-01-01")]
    [TestCase("[2025-01-01,2024-09-10[", "2024-06-10")]
    public void check_non_civilized_contains_point_in_time_throws(string periodAsString, string d)
    {
        IPeriod<T> period = ConvertStringToPeriod(periodAsString);
        T pointInTime = StringToPoint(d);
        Assert.That(() => period.Contains(pointInTime), Throws.TypeOf<ProgrammingError>());
    }

    [TestCase("[null,null[", "[2024-09-10,2025-01-01[", ExpectedResult = true)]
    [TestCase("[2024-09-10,null[", "[2024-09-10,2025-01-01[", ExpectedResult = true)]
    [TestCase("[2024-09-10,null[", "[2024-09-10,null[", ExpectedResult = true)]
    [TestCase("[2024-09-10,2025-01-01[", "[2024-09-10,2025-01-01[", ExpectedResult = true)]
    [TestCase("[2024-09-10,null[", "[2024-09-09,2025-01-01[", ExpectedResult = false)]
    [TestCase("[2024-09-10,null[", "[2024-09-09,null[", ExpectedResult = false)]
    [TestCase("[2024-09-10,null[", "[2021-09-09,2022-10-01[", ExpectedResult = false)]
    public bool check_contains_period(string periodAsString1, string periodAsString2)
    {
        IPeriod<T> period1 = ConvertStringToPeriod(periodAsString1);
        IPeriod<T> period2 = ConvertStringToPeriod(periodAsString2);
        return period1.Contains(period2);
    }

    [TestCase("[2024-09-10,2025-01-01[", "[2025-01-01,2024-09-10[")]
    [TestCase("[null,2025-01-01[", "[2025-01-01,2024-09-10[")]
    [TestCase("[2024-09-10,null[", "[2025-01-01,2024-09-10[")]
    [TestCase("[null,null[", "[2025-01-01,2024-09-10[")]
    [TestCase("[2025-01-01,2024-09-10[", "[2024-09-10,2025-01-01[")]
    [TestCase("[2025-01-01,2024-09-10[", "[2024-09-10,null[")]
    [TestCase("[2025-01-01,2024-09-10[", "[null,2025-01-01[")]
    public void check_non_civilized_contains_period_throws(string periodAsString1, string periodAsString2)
    {
        IPeriod<T> period1 = ConvertStringToPeriod(periodAsString1);
        IPeriod<T> period2 = ConvertStringToPeriod(periodAsString2);
        Assert.That(() => period1.Contains(period2), Throws.TypeOf<ProgrammingError>());
    }

    [TestCase("[null,null[", "[2024-09-10,2025-01-01[", ExpectedResult = true)]
    [TestCase("[2024-09-10,null[", "[2024-09-10,2025-01-01[", ExpectedResult = true)]
    [TestCase("[2024-09-10,null[", "[2024-09-10,null[", ExpectedResult = true)]
    [TestCase("[2024-09-10,2025-01-01[", "[2024-09-10,2025-01-01[", ExpectedResult = true)]
    [TestCase("[2024-09-10,null[", "[2024-09-09,2025-01-01[", ExpectedResult = true)]
    [TestCase("[2024-09-10,null[", "[2024-09-09,null[", ExpectedResult = true)]
    [TestCase("[2024-09-10,null[", "[2021-09-09,2022-10-01[", ExpectedResult = false)]
    public bool check_overlaps(string periodAsString1, string periodAsString2)
    {
        IPeriod<T> period1 = ConvertStringToPeriod(periodAsString1);
        IPeriod<T> period2 = ConvertStringToPeriod(periodAsString2);
        return period1.Overlaps(period2);
    }

    [TestCase("[2024-09-10,2025-01-01[", "[2025-01-01,2024-09-10[")]
    [TestCase("[null,2025-01-01[", "[2025-01-01,2024-09-10[")]
    [TestCase("[2024-09-10,null[", "[2025-01-01,2024-09-10[")]
    [TestCase("[null,null[", "[2025-01-01,2024-09-10[")]
    [TestCase("[2025-01-01,2024-09-10[", "[2024-09-10,2025-01-01[")]
    [TestCase("[2025-01-01,2024-09-10[", "[2024-09-10,null[")]
    [TestCase("[2025-01-01,2024-09-10[", "[null,2025-01-01[")]
    public void check_non_civilized_overlaps_throws(string periodAsString1, string periodAsString2)
    {
        IPeriod<T> period1 = ConvertStringToPeriod(periodAsString1);
        IPeriod<T> period2 = ConvertStringToPeriod(periodAsString2);
        Assert.That(() => period1.Overlaps(period2), Throws.TypeOf<ProgrammingError>());
    }

    [TestCase("[null,null[", "[null,null[", ExpectedResult = "[null,null[")]
    [TestCase("[null,null[", "[2025-01-01,null[", ExpectedResult = "[2025-01-01,null[")]
    [TestCase("[2024-09-10,null[", "[2025-01-01,null[", ExpectedResult = "[2025-01-01,null[")]
    [TestCase("[2024-09-10,2025-02-01[", "[2025-01-01,null[", ExpectedResult = "[2025-01-01,2025-02-01[")]
    public string check_overlapping_period(string periodAsString1, string periodAsString2)
    {
        IPeriod<T> period1 = ConvertStringToPeriod(periodAsString1);
        IPeriod<T> period2 = ConvertStringToPeriod(periodAsString2);
        return ConvertPeriodToString(period1.OverlappingPeriod(period2));
    }

    [TestCase("[2024-09-10,2025-01-01[", "[2025-01-01,2024-09-10[")]
    [TestCase("[null,2025-01-01[", "[2025-01-01,2024-09-10[")]
    [TestCase("[2024-09-10,null[", "[2025-01-01,2024-09-10[")]
    [TestCase("[null,null[", "[2025-01-01,2024-09-10[")]
    [TestCase("[2025-01-01,2024-09-10[", "[2024-09-10,2025-01-01[")]
    [TestCase("[2025-01-01,2024-09-10[", "[2024-09-10,null[")]
    [TestCase("[2025-01-01,2024-09-10[", "[null,2025-01-01[")]
    public void check_non_civilized_overlapping_period_throws(string periodAsString1, string periodAsString2)
    {
        IPeriod<T> period1 = ConvertStringToPeriod(periodAsString1);
        IPeriod<T> period2 = ConvertStringToPeriod(periodAsString2);
        Assert.That(() => period1.OverlappingPeriod(period2), Throws.TypeOf<ProgrammingError>());
    }

    [TestCase("[null,null[", "[null,null[", ExpectedResult = true)]
    [TestCase("[2025-01-01,null[", "[null,null[", ExpectedResult = true)]
    [TestCase("[2025-01-01,2025-02-01[", "[null,null[", ExpectedResult = true)]
    [TestCase("[2025-01-01,2025-02-01[", "[2025-01-01,2025-02-01[", ExpectedResult = true)]
    [TestCase("[2025-01-01,2025-02-01[", "[2024-01-01,2025-02-01[", ExpectedResult = true)]
    [TestCase("[2025-01-01,2025-02-01[", "[2025-01-01,2026-02-01[", ExpectedResult = true)]
    [TestCase("[2025-01-01,2025-02-01[", "[2024-01-01,2026-02-01[", ExpectedResult = true)]
    [TestCase("[null,null[", "[2025-01-01,null[", ExpectedResult = false)]
    public bool check_is_completely_contained_within(string periodAsString1, string periodAsString2)
    {
        IPeriod<T> period1 = ConvertStringToPeriod(periodAsString1);
        IPeriod<T> period2 = ConvertStringToPeriod(periodAsString2);
        return period1.IsCompletelyContainedWithin(period2);
    }

    [TestCase("[2024-09-10,2025-01-01[", "[2025-01-01,2024-09-10[")]
    [TestCase("[null,2025-01-01[", "[2025-01-01,2024-09-10[")]
    [TestCase("[2024-09-10,null[", "[2025-01-01,2024-09-10[")]
    [TestCase("[null,null[", "[2025-01-01,2024-09-10[")]
    [TestCase("[2025-01-01,2024-09-10[", "[2024-09-10,2025-01-01[")]
    [TestCase("[2025-01-01,2024-09-10[", "[2024-09-10,null[")]
    [TestCase("[2025-01-01,2024-09-10[", "[null,2025-01-01[")]
    public void check_non_civilized_is_completely_contained_within_throws(string periodAsString1, string periodAsString2)
    {
        IPeriod<T> period1 = ConvertStringToPeriod(periodAsString1);
        IPeriod<T> period2 = ConvertStringToPeriod(periodAsString2);
        Assert.That(() => period1.IsCompletelyContainedWithin(period2), Throws.TypeOf<ProgrammingError>());
    }
}
