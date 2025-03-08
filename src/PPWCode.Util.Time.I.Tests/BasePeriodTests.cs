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

using System.Text;
using System.Text.RegularExpressions;

using PPWCode.Vernacular.Contracts.I;
using PPWCode.Vernacular.Exceptions.V;

namespace PPWCode.Util.Time.I.Tests;

public abstract class BasePeriodTests<TPeriod, T> : BaseFixture
    where TPeriod : class, IPeriod<T>
    where T : struct, IComparable<T>, IEquatable<T>
{
    private const string NullString = "null";

    protected abstract TPeriod CreatePeriod(T? from, T? to);
    protected abstract T StringToPoint(string value);
    protected abstract string PointToString(T value);

    protected abstract T CreatePoint(int year, int month, int day);
    protected abstract T AddToPoint(T date, int i);

    protected virtual string ConvertPointToString(T? value)
        => value == null
               ? NullString
               : PointToString(value.Value);

    protected virtual T? ConvertStringToPoint(string value)
        => string.Equals(value, NullString, StringComparison.InvariantCultureIgnoreCase)
               ? null
               : StringToPoint(value);

    /// <summary>
    ///     Regex that must have 2 named groups: point1 to refer to the start point
    ///     and point2 to refer to the end point.
    /// </summary>
    protected abstract Regex PeriodRegex { get; }

    protected virtual TPeriod ConvertStringToPeriod(string value)
    {
        if (!string.IsNullOrWhiteSpace(value))
        {
            Match m = PeriodRegex.Match(value);
            if (m.Success)
            {
                T? from = ConvertStringToPoint(m.Groups["point1"].Value);
                T? to = ConvertStringToPoint(m.Groups["point2"].Value);
                return CreatePeriod(from, to);
            }
        }

        throw new ArgumentException($"'{value}' is not a valid period string");
    }

    protected virtual string ConvertPeriodToString(IPeriod<T> period)
        => $"[{ConvertPointToString(period.From)},{ConvertPointToString(period.To)}[";

    // _ => empty
    // X => part of interval (period = 1 month)
    // . => minus or plus infinity (depending on first or last character)
    //
    // "___XXXX__XXXXX_XXXXX_XXXXX_XXXX."
    // "___X__X__XXXXX_XXXXX_XXXXX_XXXX_"
    public TPeriod[] ConvertStringToPeriods(T startDate, string intervalString)
    {
        ReadOnlySpan<char> stateChars = intervalString.AsSpan();
        Stack<TPeriod> periods = new ();
        T currentDate = startDate;
        TPeriod? current = null;
        char? previous = null;

        foreach (char c in stateChars)
        {
            switch (c)
            {
                case 'X':
                    current ??= CreatePeriod(currentDate, null);

                    break;
                case '_':
                    if (current != null)
                    {
                        periods.Push(CreatePeriod(current.From, currentDate));
                        current = null;
                    }

                    break;
                case '.':
                    if (previous == null)
                    {
                        Contract.Assert(current == null);
                        current = CreatePeriod(null, null);
                    }
                    else
                    {
                        current = CreatePeriod(current == null ? currentDate : current.From, null);
                    }

                    break;
                default:
                    throw new InternalProgrammingError("Invalid period string!");
            }

            currentDate = AddToPoint(currentDate, 1);
            previous = c;
        }

        if (current != null)
        {
            periods.Push(CreatePeriod(current.From, previous == '.' ? null : currentDate));
        }

        return periods.ToArray();
    }

    protected string ConvertPeriodsToString(T startDate, IEnumerable<IPeriod<T>> periods)
    {
        // empty, no periods
        if (!periods.Any())
        {
            return string.Empty;
        }

        // convert to linked list
        LinkedList<IPeriod<T>> linkedPeriods = new (periods.OrderBy(p => p.CoalesceFrom));
        LinkedListNode<IPeriod<T>>? firstNode = linkedPeriods.First;
        LinkedListNode<IPeriod<T>>? lastNode = linkedPeriods.Last;

        Contract.Assert(firstNode != null);
        Contract.Assert(lastNode != null);

        // loop through linked list
        T? endDate = lastNode.Value.To;
        StringBuilder sb = new ();
        for (T date = startDate; endDate is null || (date.CompareTo(endDate.Value) < 0); date = AddToPoint(date, 1))
        {
            IPeriod<T>? period = linkedPeriods.FirstOrDefault(p => p.Contains(date));
            if (period is { From: null } && (firstNode.Value == period))
            {
                sb.Append('.');
                break;
            }

            if (endDate is null && (lastNode.Value == period))
            {
                sb.Append('.');
                break;
            }

            sb.Append(period is not null ? "X" : "_");
        }

        return sb.ToString();
    }

    protected string CanonicalizePeriodsString(string periodsString)
    {
        string handlePlusInfinity =
            Regex.Replace(
                periodsString,
                @"^((?<start>X|\.?[_X]*_)|\.)(X+\.)$",
                "${start}.",
                RegexOptions.Compiled);
        string handleTrailingEmpty =
            Regex.Replace(
                handlePlusInfinity,
                @"^(?<start>\._|\.?[_X]*X|)(_+)$",
                "${start}",
                RegexOptions.Compiled);
        return handleTrailingEmpty;
    }
}
