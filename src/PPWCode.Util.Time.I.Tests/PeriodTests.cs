using System.Text.RegularExpressions;

using NUnit.Framework;

namespace PPWCode.Util.Time.I.Tests;

[TestFixture]
public abstract class PeriodTests<T>
    where T : struct, IComparable<T>, IEquatable<T>
{
    // ReSharper disable once StaticMemberInGenericType
    private static readonly Regex _regex = new (@"^\[\s*(?:(\d{4}-\d{1,2}-\d{1,2})|null)\s*,\s*(?:(\d{4}-\d{1,2}-\d{1,2})|null)\s*\[$");
    protected abstract IPeriod<T> Create(T? from, T? to);
    protected abstract T? ConvertFromString(string? value);
    protected abstract string? ConvertToString(T? value);

    protected virtual IPeriod<T> ConvertFromStringPeriod(string? value)
    {
        if (!string.IsNullOrWhiteSpace(value))
        {
            Match m = _regex.Match(value);
            if (m.Success)
            {
                T? from = ConvertFromString(m.Groups[1].Value);
                T? to = ConvertFromString(m.Groups[2].Value);
                return Create(from, to);
            }
        }

        throw new ArgumentException($"'{value}' is not a valid period string");
    }

    protected virtual string? ConvertToString(IPeriod<T>? period)
    {
        if (period is null)
        {
            return null;
        }

        return $"[{ConvertToString(period.From)},{ConvertToString(period.To)}[";
    }

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
