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
    protected override PeriodMultiHistory<DateOnlyPeriod, DateOnly> CreateMultiPeriodHistory(IEnumerable<IPeriod<DateOnly>> periods)
        => new DateOnlyPeriodMultiHistory(periods.Select(p => CreatePeriod(p.From, p.To)));
}
