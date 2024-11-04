using NUnit.Framework;

using PPWCode.Vernacular.Exceptions.V;

namespace PPWCode.Util.Time.I.Tests;

[TestFixture]
public abstract class PeriodTests<TPeriod, T> : BasePeriodTests<TPeriod, T>
    where TPeriod : class, IPeriod<T>
    where T : struct, IComparable<T>, IEquatable<T>
{
    public virtual bool check_contains_point_in_time(string periodAsString, string d)
    {
        IPeriod<T> period = ConvertStringToPeriod(periodAsString);
        T pointInTime = StringToPoint(d);
        return period.Contains(pointInTime);
    }

    public virtual void check_non_civilized_contains_point_in_time_throws(string periodAsString, string d)
    {
        IPeriod<T> period = ConvertStringToPeriod(periodAsString);
        T pointInTime = StringToPoint(d);
        Assert.That(() => period.Contains(pointInTime), Throws.TypeOf<ProgrammingError>());
    }

    public virtual bool check_contains_period(string periodAsString1, string periodAsString2)
    {
        IPeriod<T> period1 = ConvertStringToPeriod(periodAsString1);
        IPeriod<T> period2 = ConvertStringToPeriod(periodAsString2);
        return period1.Contains(period2);
    }

    public virtual void check_non_civilized_contains_period_throws(string periodAsString1, string periodAsString2)
    {
        IPeriod<T> period1 = ConvertStringToPeriod(periodAsString1);
        IPeriod<T> period2 = ConvertStringToPeriod(periodAsString2);
        Assert.That(() => period1.Contains(period2), Throws.TypeOf<ProgrammingError>());
    }

    public virtual bool check_overlaps(string periodAsString1, string periodAsString2)
    {
        IPeriod<T> period1 = ConvertStringToPeriod(periodAsString1);
        IPeriod<T> period2 = ConvertStringToPeriod(periodAsString2);
        return period1.Overlaps(period2);
    }

    public virtual void check_non_civilized_overlaps_throws(string periodAsString1, string periodAsString2)
    {
        IPeriod<T> period1 = ConvertStringToPeriod(periodAsString1);
        IPeriod<T> period2 = ConvertStringToPeriod(periodAsString2);
        Assert.That(() => period1.Overlaps(period2), Throws.TypeOf<ProgrammingError>());
    }

    public virtual string check_overlapping_period(string periodAsString1, string periodAsString2)
    {
        IPeriod<T> period1 = ConvertStringToPeriod(periodAsString1);
        IPeriod<T> period2 = ConvertStringToPeriod(periodAsString2);
        return ConvertPeriodToString(period1.OverlappingPeriod(period2));
    }

    public virtual void check_non_civilized_overlapping_period_throws(string periodAsString1, string periodAsString2)
    {
        IPeriod<T> period1 = ConvertStringToPeriod(periodAsString1);
        IPeriod<T> period2 = ConvertStringToPeriod(periodAsString2);
        Assert.That(() => period1.OverlappingPeriod(period2), Throws.TypeOf<ProgrammingError>());
    }

    public virtual bool check_is_completely_contained_within(string periodAsString1, string periodAsString2)
    {
        IPeriod<T> period1 = ConvertStringToPeriod(periodAsString1);
        IPeriod<T> period2 = ConvertStringToPeriod(periodAsString2);
        return period1.IsCompletelyContainedWithin(period2);
    }

    public virtual void check_non_civilized_is_completely_contained_within_throws(string periodAsString1, string periodAsString2)
    {
        IPeriod<T> period1 = ConvertStringToPeriod(periodAsString1);
        IPeriod<T> period2 = ConvertStringToPeriod(periodAsString2);
        Assert.That(() => period1.IsCompletelyContainedWithin(period2), Throws.TypeOf<ProgrammingError>());
    }
}
