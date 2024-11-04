using System.Text;
using System.Text.RegularExpressions;

using NUnit.Framework;

using PPWCode.Vernacular.Exceptions.V;

namespace PPWCode.Util.Time.I.Tests;

public abstract class BasePeriodTests<TPeriod, T> : BaseFixture
    where TPeriod : class, IPeriod<T>
    where T : struct, IComparable<T>, IEquatable<T>
{
    private const string NullString = "null";

    // ReSharper disable once StaticMemberInGenericType
    private static readonly Regex _regex = new (@"^\[\s*(\d{4}-\d{1,2}-\d{1,2}|null)\s*,\s*(\d{4}-\d{1,2}-\d{1,2}|null)\s*\[$");
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

    protected virtual TPeriod ConvertStringToPeriod(string value)
    {
        if (!string.IsNullOrWhiteSpace(value))
        {
            Match m = _regex.Match(value);
            if (m.Success)
            {
                T? from = ConvertStringToPoint(m.Groups[1].Value);
                T? to = ConvertStringToPoint(m.Groups[2].Value);
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
        char[] stateChars = intervalString.ToCharArray();
        Stack<TPeriod> periods = new ();

        T currentDate = startDate;
        TPeriod? current = null;
        foreach (char c in stateChars)
        {
            switch (c)
            {
                case 'X':
                    if (current == null)
                    {
                        current = CreatePeriod(currentDate, null);
                        periods.Push(current);
                    }

                    break;
                case '_':
                    if (current != null)
                    {
                        periods.Pop();
                        periods.Push(CreatePeriod(current.From, currentDate));
                        current = null;
                    }

                    break;
                case '.':
                    if (current != null)
                    {
                        // ends with dot
                        periods.Pop();
                        periods.Push(CreatePeriod(current.From, null));
                        current = null;
                    }
                    else
                    {
                        // starts with dot
                        current = CreatePeriod(null, null);
                        periods.Push(current);
                    }

                    break;
                default:
                    throw new InternalProgrammingError("Invalid period string!");
            }

            currentDate = AddToPoint(currentDate, 1);
        }

        return periods.ToArray();
    }

    protected string ConvertPeriodsToString(T startDate, IEnumerable<TPeriod> periods)
    {
        LinkedList<TPeriod> linkedPeriods = new (periods.OrderBy(p => p.CoalesceFrom));
        LinkedListNode<TPeriod>? firstNode = linkedPeriods.First;
        LinkedListNode<TPeriod>? lastNode = linkedPeriods.Last;

        if (firstNode is null)
        {
            return string.Empty;
        }

        if (firstNode.Value.From is null)
        {
            Assert.That(false, "Infinitive periods aren't supported");
        }

        // when we have a first node, we definitively have a last node
        T? endDate = lastNode!.Value.To;
        StringBuilder sb = new ();
        for (T date = startDate; endDate is null || (date.CompareTo(endDate.Value) < 0); date = AddToPoint(date, 1))
        {
            TPeriod? period = linkedPeriods.FirstOrDefault(p => p.Contains(date));
            if (endDate is null && (lastNode.Value == period))
            {
                sb.Append('.');
                break;
            }

            sb.Append(period is not null ? "X" : "_");
        }

        return sb.ToString();
    }
}
