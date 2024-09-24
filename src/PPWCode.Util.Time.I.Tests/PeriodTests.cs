using NUnit.Framework;

namespace PPWCode.Util.Time.I.Tests;

[TestFixture]
public abstract class PeriodTests<TPeriod, T> : BasePeriodTests<TPeriod, T>
    where TPeriod : class, IPeriod<T>
    where T : struct, IComparable<T>, IEquatable<T>
{
    [TestCase("[null,null[", "2025-01-01", ExpectedResult = true)]
    [TestCase("[2024-09-10,null[", "2025-01-01", ExpectedResult = true)]
    [TestCase("[2024-09-10,null[", null, ExpectedResult = false)]
    [TestCase("[2024-09-10,2025-01-01[", "2025-01-01", ExpectedResult = false)]
    [TestCase("[2024-09-10,null[", "2024-01-01", ExpectedResult = false)]
    public bool check_contains_point_in_time(string periodAsString, string? d)
    {
        IPeriod<T> period = ConvertFromStringPeriod(periodAsString);
        T? pointInTime = ConvertFromString(d);
        return period.Contains(pointInTime);
    }

    [TestCase("[null,null[", "[2024-09-10,2025-01-01[", ExpectedResult = true)]
    [TestCase("[2024-09-10,null[", "[2024-09-10,2025-01-01[", ExpectedResult = true)]
    [TestCase("[2024-09-10, null[", "[2024-09-10,null[", ExpectedResult = true)]
    [TestCase("[2024-09-10,2025-01-01[", "[2024-09-10,2025-01-01[", ExpectedResult = true)]
    [TestCase("[2024-09-10, null[", "[2024-09-09,2025-01-01[", ExpectedResult = false)]
    [TestCase("[2024-09-10, null[", "[2024-09-09,null[", ExpectedResult = false)]
    [TestCase("[2024-09-10, null[", "[2021-09-09,2022-10-01[", ExpectedResult = false)]
    public bool check_contains_period(string periodAsString1, string periodAsString2)
    {
        IPeriod<T> period1 = ConvertFromStringPeriod(periodAsString1);
        IPeriod<T> period2 = ConvertFromStringPeriod(periodAsString2);
        return period1.Contains(period2);
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
        IPeriod<T> period1 = ConvertFromStringPeriod(periodAsString1);
        IPeriod<T> period2 = ConvertFromStringPeriod(periodAsString2);
        return period1.Overlaps(period2);
    }

    [TestCase("[null,null[", "[null,null[", ExpectedResult = "[null,null[")]
    [TestCase("[null,null[", "[2025-01-01,null[", ExpectedResult = "[2025-01-01,null[")]
    [TestCase("[2024-09-10,null[", "[2025-01-01,null[", ExpectedResult = "[2025-01-01,null[")]
    [TestCase("[2024-09-10,2025-02-01[", "[2025-01-01,null[", ExpectedResult = "[2025-01-01,2025-02-01[")]
    public string? check_overlapping_period(string periodAsString1, string periodAsString2)
    {
        IPeriod<T> period1 = ConvertFromStringPeriod(periodAsString1);
        IPeriod<T> period2 = ConvertFromStringPeriod(periodAsString2);
        return ConvertToString(period1.OverlappingPeriod(period2));
    }
}
