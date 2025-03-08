// Copyright 2025 by PeopleWare n.v..
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// http://www.apache.org/licenses/LICENSE-2.0
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

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
