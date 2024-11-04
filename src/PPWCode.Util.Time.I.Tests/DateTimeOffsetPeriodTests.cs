using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;

using NUnit.Framework;

namespace PPWCode.Util.Time.I.Tests;

public class DateTimeOffsetPeriodTests : PeriodTests<DateTimeOffsetPeriod, DateTimeOffset>
{
    /// <inheritdoc />
    protected override DateTimeOffsetPeriod CreatePeriod(DateTimeOffset? from, DateTimeOffset? to)
        => new (from, to);

    /// <inheritdoc />
    protected override DateTimeOffset StringToPoint(string value)
        => DateTimeOffset.Parse(value);

    /// <inheritdoc />
    protected override string PointToString(DateTimeOffset value)
        => $"{value:yyyy'-'MM'-'dd'T'HH':'mm':'sszzz}";

    /// <inheritdoc />
    protected override DateTimeOffset CreatePoint(int year, int month, int day)
        => new (year, month, day, 2, 25, 37, TimeSpan.FromHours(-7));

    /// <inheritdoc />
    protected override DateTimeOffset AddToPoint(DateTimeOffset point, int i)
        => point.AddHours(30);

    protected override Regex PeriodRegex
        => new (@"^\[(?<point1>\d{4}-\d{2}-\d{2}T\d{2}:\d{2}:\d{2}(\-|\+)\d{2}:\d{2}|null),(?<point2>\d{4}-\d{2}-\d{2}T\d{2}:\d{2}:\d{2}(\-|\+)\d{2}:\d{2}|null)\[$", RegexOptions.Compiled);

    [Test]
    public void check_point_to_string_to_point_1()
    {
        DateTimeOffset point = new (2024, 10, 23, 17, 41, 57, TimeSpan.FromHours(-7));
        string display = "2024-10-23T17:41:57-07:00";
        Assert.That(PointToString(point), Is.EqualTo(display));
        Assert.That(StringToPoint(display), Is.EqualTo(point));
    }

    [Test]
    public void check_point_to_string_to_point_2()
    {
        DateTimeOffset point = new (2024, 3, 4, 8, 9, 2, TimeSpan.FromHours(2.5));
        string display = "2024-03-04T08:09:02+02:30";
        Assert.That(PointToString(point), Is.EqualTo(display));
        Assert.That(StringToPoint(display), Is.EqualTo(point));
    }

    [Test]
    [SuppressMessage("ReSharper", "EqualExpressionComparison", Justification = "Tests")]
    [SuppressMessage("ReSharper", "ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract", Justification = "Tests")]
    public void check_equals_1()
    {
        string p = "[2024-03-04T08:09:02+02:30,2024-10-23T17:41:57-07:00[";
        DateTimeOffsetPeriod period = ConvertStringToPeriod(p);
        Assert.That(() => period.Equals(period), Is.True);
#pragma warning disable CS1718 // Comparison made to same variable
        Assert.That(() => period == period, Is.True);
        Assert.That(() => period != period, Is.False);
#pragma warning restore CS1718 // Comparison made to same variable
        Assert.That(() => period.Equals(null), Is.False);
        Assert.That(() => period == null, Is.False);
        Assert.That(() => period != null, Is.True);
        Assert.That(() => period.Equals(null), Is.False);
    }

    [Test]
    [SuppressMessage("ReSharper", "SuspiciousTypeConversion.Global", Justification = "Tests")]
    public void check_equals_2()
    {
        string p = "[2024-03-04T00:00:00+00:00,2024-10-23T00:00:00+00:00[";
        DateTimeOffsetPeriod period = ConvertStringToPeriod(p);
        DateOnlyPeriod datePeriod = new DateOnlyPeriod(new DateOnly(2024, 3, 4), new DateOnly(2024, 10, 23));
        Assert.That(() => period.Equals(datePeriod), Is.False);
        Assert.That(() => datePeriod.Equals(period), Is.False);
    }

    [Test]
    public void check_equals_3()
    {
        string p1 = "[2024-03-04T08:09:02+02:30,2024-10-23T17:41:57-07:00[";
        string p2 = "[2024-03-04T06:39:02+01:00,2024-10-23T19:41:57-05:00[";
        DateTimeOffsetPeriod period1 = ConvertStringToPeriod(p1);
        DateTimeOffsetPeriod period2 = ConvertStringToPeriod(p2);
        Assert.That(() => period1.Equals(period2), Is.True);
        Assert.That(() => period2.Equals(period1), Is.True);
        Assert.That(() => period1 == period2, Is.True);
        Assert.That(() => period2 == period1, Is.True);
    }

    [Test]
    public void check_equals_4()
    {
        string p1 = "[2024-03-04T08:09:02+02:30,2024-10-23T17:41:57-07:00[";
        string p2 = "[2024-03-04T08:09:02+02:30,2024-10-23T17:41:57-05:00[";
        DateTimeOffsetPeriod period1 = ConvertStringToPeriod(p1);
        DateTimeOffsetPeriod period2 = ConvertStringToPeriod(p2);
        Assert.That(() => period1.Equals(period2), Is.False);
        Assert.That(() => period2.Equals(period1), Is.False);
        Assert.That(() => period1 == period2, Is.False);
        Assert.That(() => period2 == period1, Is.False);
    }

    [TestCase("[null,null[", "2024-03-04T08:09:02+02:30", ExpectedResult = true)]
    [TestCase("[2024-03-04T08:09:02+02:30,null[", "2024-10-23T17:41:57-07:00", ExpectedResult = true)]
    [TestCase("[2024-03-04T08:09:02+02:30,null[", "2024-03-04T09:09:02-02:30", ExpectedResult = true)]
    [TestCase("[2024-03-04T08:09:02+02:30,null[", "2024-03-04T08:09:02+03:00", ExpectedResult = false)]
    [TestCase("[2024-03-04T08:09:02+02:30,2024-10-23T17:41:57-07:00[", "2024-10-23T17:41:57-07:00", ExpectedResult = false)]
    [TestCase("[2024-10-23T17:41:57-07:00,null[", "2024-03-04T08:09:02+02:30", ExpectedResult = false)]
    [TestCase("[2024-10-23T17:41:57-07:00,null[", "2024-10-23T17:42:00-07:00", ExpectedResult = true)]
    [TestCase("[2024-10-23T17:41:57-07:00,null[", "2024-10-23T17:41:51-08:00", ExpectedResult = true)]
    [TestCase("[2024-10-23T17:41:57-07:00,2024-10-23T17:41:59-07:00[", "2024-10-23T17:41:58-07:00", ExpectedResult = true)]
    [TestCase("[2024-10-23T17:41:57-07:00,2024-10-23T17:41:59-07:00[", "2024-10-23T17:41:59-07:00", ExpectedResult = false)]
    public override bool check_contains_point_in_time(string periodAsString, string d)
        => base.check_contains_point_in_time(periodAsString, d);

    [TestCase("[2024-10-23T17:41:57-07:00,2024-03-04T08:09:02+02:30[", "2024-10-23T17:41:57-07:00")]
    [TestCase("[2024-10-23T17:41:57-07:00,2024-03-04T08:09:02+02:30[", "2024-02-23T13:21:54+00:00")]
    public override void check_non_civilized_contains_point_in_time_throws(string periodAsString, string d)
        => base.check_non_civilized_contains_point_in_time_throws(periodAsString, d);

    [TestCase("[null,null[", "[2024-03-04T08:09:02+02:30,2024-10-23T17:41:57-07:00[", ExpectedResult = true)]
    [TestCase("[2024-03-04T08:09:02+02:30,null[", "[2024-03-04T08:09:02+02:30,2024-10-23T17:41:57-07:00[", ExpectedResult = true)]
    [TestCase("[2024-03-04T08:09:02+02:30,null[", "[2024-03-04T08:09:02+02:30,null[", ExpectedResult = true)]
    [TestCase("[2024-03-04T08:09:02+02:30,2024-10-23T17:41:57-07:00[", "[2024-03-04T08:09:02+02:30,2024-10-23T17:41:57-07:00[", ExpectedResult = true)]
    [TestCase("[2024-03-04T08:09:02+02:30,null[", "[2024-03-04T08:09:01+02:30,2024-10-23T17:41:57-07:00[", ExpectedResult = false)]
    [TestCase("[2024-03-04T08:09:02+02:30,null[", "[2024-03-04T08:09:00+02:30,null[", ExpectedResult = false)]
    [TestCase("[2024-03-04T08:09:02+02:30,null[", "[2024-01-29T23:14:03+02:00,2024-02-13T18:21:06+02:00[", ExpectedResult = false)]
    [TestCase("[2024-03-04T08:09:02+02:30,2024-03-04T08:09:42+02:30[", "[2024-03-04T08:09:12+02:30,2024-03-04T08:09:32+02:30[", ExpectedResult = true)]
    [TestCase("[2024-03-04T08:09:12+02:30,2024-03-04T08:09:32+02:30[", "[2024-03-04T08:09:02+02:30,2024-03-04T08:09:42+02:30[", ExpectedResult = false)]
    public override bool check_contains_period(string periodAsString1, string periodAsString2)
        => base.check_contains_period(periodAsString1, periodAsString2);

    [TestCase("[2024-03-04T08:09:02+02:30,2024-10-23T17:41:57-07:00[", "[2024-10-23T17:41:57-07:00,2024-03-04T08:09:02+02:30[")]
    [TestCase("[null,2024-10-23T17:41:57-07:00[", "[2024-10-23T17:41:57-07:00,2024-03-04T08:09:02+02:30[")]
    [TestCase("[2024-03-04T08:09:02+02:30,null[", "[2024-10-23T17:41:57-07:00,2024-03-04T08:09:02+02:30[")]
    [TestCase("[null,null[", "[2024-10-23T17:41:57-07:00,2024-03-04T08:09:02+02:30[")]
    [TestCase("[2024-10-23T17:41:57-07:00,2024-03-04T08:09:02+02:30[", "[2024-03-04T08:09:02+02:30,2024-10-23T17:41:57-07:00[")]
    [TestCase("[2024-10-23T17:41:57-07:00,2024-03-04T08:09:02+02:30[", "[2024-03-04T08:09:02+02:30,null[")]
    [TestCase("[2024-10-23T17:41:57-07:00,2024-03-04T08:09:02+02:30[", "[null,2024-10-23T17:41:57-07:00[")]
    public override void check_non_civilized_contains_period_throws(string periodAsString1, string periodAsString2)
        => base.check_non_civilized_contains_period_throws(periodAsString1, periodAsString2);

    [TestCase("[null,null[", "[2024-03-04T08:09:02+02:30,2024-10-23T17:41:57-07:00[", ExpectedResult = true)]
    [TestCase("[2024-03-04T08:09:02+02:30,null[", "[2024-03-04T08:09:02+02:30,2024-10-23T17:41:57-07:00[", ExpectedResult = true)]
    [TestCase("[2024-03-04T08:09:02+02:30,null[", "[2024-03-04T08:09:02+02:30,null[", ExpectedResult = true)]
    [TestCase("[2024-03-04T08:09:02+02:30,2024-10-23T17:41:57-07:00[", "[2024-03-04T08:09:02+02:30,2024-10-23T17:41:57-07:00[", ExpectedResult = true)]
    [TestCase("[2024-03-04T08:09:02+02:30,null[", "[2024-03-04T08:05:02+02:30,2024-10-23T17:41:57-07:00[", ExpectedResult = true)]
    [TestCase("[2024-03-04T08:09:02+02:30,null[", "[2024-03-04T08:05:02+02:30,null[", ExpectedResult = true)]
    [TestCase("[2024-03-04T08:09:02+02:30,null[", "[2024-03-04T07:05:02+02:30,2024-03-04T08:08:02+02:30[", ExpectedResult = false)]
    public override bool check_overlaps(string periodAsString1, string periodAsString2)
        => base.check_overlaps(periodAsString1, periodAsString2);

    [TestCase("[2024-09-10T08:09:02+01:00,2025-01-01T16:23:45+02:00[", "[2025-01-01T16:23:45+02:00,2024-09-10T08:09:02+01:00[")]
    [TestCase("[null,2025-01-01T16:23:45+02:00[", "[2025-01-01T16:23:45+02:00,2024-09-10T08:09:02+01:00[")]
    [TestCase("[2024-09-10T08:09:02+01:00,null[", "[2025-01-01T16:23:45+02:00,2024-09-10T08:09:02+01:00[")]
    [TestCase("[null,null[", "[2025-01-01T16:23:45+02:00,2024-09-10T08:09:02+01:00[")]
    [TestCase("[2025-01-01T16:23:45+02:00,2024-09-10T08:09:02+01:00[", "[2024-09-10T08:09:02+01:00,2025-01-01T16:23:45+02:00[")]
    [TestCase("[2025-01-01T16:23:45+02:00,2024-09-10T08:09:02+01:00[", "[2024-09-10T08:09:02+01:00,null[")]
    [TestCase("[2025-01-01T16:23:45+02:00,2024-09-10T08:09:02+01:00[", "[null,2025-01-01T16:23:45+02:00[")]
    public override void check_non_civilized_overlaps_throws(string periodAsString1, string periodAsString2)
        => base.check_non_civilized_overlaps_throws(periodAsString1, periodAsString2);

    [TestCase("[null,null[", "[null,null[", ExpectedResult = "[null,null[")]
    [TestCase("[null,null[", "[2025-01-01T16:23:45+02:00,null[", ExpectedResult = "[2025-01-01T16:23:45+02:00,null[")]
    [TestCase("[2024-09-10T08:09:02+01:00,null[", "[2025-01-01T16:23:45+02:00,null[", ExpectedResult = "[2025-01-01T16:23:45+02:00,null[")]
    [TestCase("[2024-09-10T08:09:02+01:00,2025-02-01T13:42:13+02:00[", "[2025-01-01T16:23:45+02:00,null[", ExpectedResult = "[2025-01-01T16:23:45+02:00,2025-02-01T13:42:13+02:00[")]
    public override string check_overlapping_period(string periodAsString1, string periodAsString2)
        => base.check_overlapping_period(periodAsString1, periodAsString2);

    [TestCase("[2024-09-10T08:09:02+01:00,2025-01-01T13:42:13+02:00[", "[2025-01-01T13:42:13+02:00,2024-09-10T08:09:02+01:00[")]
    [TestCase("[null,2025-01-01T13:42:13+02:00[", "[2025-01-01T13:42:13+02:00,2024-09-10T08:09:02+01:00[")]
    [TestCase("[2024-09-10T08:09:02+01:00,null[", "[2025-01-01T13:42:13+02:00,2024-09-10T08:09:02+01:00[")]
    [TestCase("[null,null[", "[2025-01-01T13:42:13+02:00,2024-09-10T08:09:02+01:00[")]
    [TestCase("[2025-01-01T13:42:13+02:00,2024-09-10T08:09:02+01:00[", "[2024-09-10T08:09:02+01:00,2025-01-01T13:42:13+02:00[")]
    [TestCase("[2025-01-01T13:42:13+02:00,2024-09-10T08:09:02+01:00[", "[2024-09-10T08:09:02+01:00,null[")]
    [TestCase("[2025-01-01T13:42:13+02:00,2024-09-10T08:09:02+01:00[", "[null,2025-01-01T13:42:13+02:00[")]
    public override void check_non_civilized_overlapping_period_throws(string periodAsString1, string periodAsString2)
        => base.check_non_civilized_overlapping_period_throws(periodAsString1, periodAsString2);

    [TestCase("[null,null[", "[null,null[", ExpectedResult = true)]
    [TestCase("[2025-01-01T13:42:13+02:00,null[", "[null,null[", ExpectedResult = true)]
    [TestCase("[2025-01-01T13:42:13+02:00,2025-02-01T04:39:51+03:00[", "[null,null[", ExpectedResult = true)]
    [TestCase("[2025-01-01T13:42:13+02:00,2025-02-01T04:39:51+03:00[", "[2025-01-01T13:42:13+02:00,2025-02-01T04:39:51+03:00[", ExpectedResult = true)]
    [TestCase("[2025-01-01T13:42:13+02:00,2025-02-01T04:39:51+03:00[", "[2024-01-01T22:33:44-07:00,2025-02-01T04:39:51+03:00[", ExpectedResult = true)]
    [TestCase("[2025-01-01T13:42:13+02:00,2025-02-01T04:39:51+03:00[", "[2025-01-01T13:42:13+02:00,2026-02-01T07:41:17+02:00[", ExpectedResult = true)]
    [TestCase("[2025-01-01T13:42:13+02:00,2025-02-01T04:39:51+03:00[", "[2024-01-01T15:06:54+02:30,2026-02-01T07:41:17+02:00[", ExpectedResult = true)]
    [TestCase("[null,null[", "[2025-01-01T22:33:44-07:00,null[", ExpectedResult = false)]
    public override bool check_is_completely_contained_within(string periodAsString1, string periodAsString2)
        => base.check_is_completely_contained_within(periodAsString1, periodAsString2);

    [TestCase("[2024-09-10T13:42:13+02:00,2025-01-01T04:39:51+03:00[", "[2025-01-01T04:39:51+03:00,2024-09-10T13:42:13+02:00[")]
    [TestCase("[null,2025-01-01T04:39:51+03:00[", "[2025-01-01T04:39:51+03:00,2024-09-10T13:42:13+02:00[")]
    [TestCase("[2024-09-10T13:42:13+02:00,null[", "[2025-01-01T04:39:51+03:00,2024-09-10T13:42:13+02:00[")]
    [TestCase("[null,null[", "[2025-01-01T04:39:51+03:00,2024-09-10T13:42:13+02:00[")]
    [TestCase("[2025-01-01T04:39:51+03:00,2024-09-10T13:42:13+02:00[", "[2024-09-10T13:42:13+02:00,2025-01-01T04:39:51+03:00[")]
    [TestCase("[2025-01-01T04:39:51+03:00,2024-09-10T13:42:13+02:00[", "[2024-09-10T13:42:13+02:00,null[")]
    [TestCase("[2025-01-01T04:39:51+03:00,2024-09-10T13:42:13+02:00[", "[null,2025-01-01T04:39:51+03:00[")]
    public override void check_non_civilized_is_completely_contained_within_throws(string periodAsString1, string periodAsString2)
        => base.check_non_civilized_is_completely_contained_within_throws(periodAsString1, periodAsString2);
}
