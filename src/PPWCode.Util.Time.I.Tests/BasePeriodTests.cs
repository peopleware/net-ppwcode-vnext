using System.Text.RegularExpressions;

using PPWCode.Vernacular.Exceptions.V;

namespace PPWCode.Util.Time.I.Tests;

public abstract class BasePeriodTests<TPeriod, T> : BaseFixture
    where TPeriod : class, IPeriod<T>
    where T : struct, IComparable<T>, IEquatable<T>
{
    // ReSharper disable once StaticMemberInGenericType
    private static readonly Regex _regex = new (@"^\[\s*(?:(\d{4}-\d{1,2}-\d{1,2})|null)\s*,\s*(?:(\d{4}-\d{1,2}-\d{1,2})|null)\s*\[$");
    protected abstract TPeriod CreatePeriod(T? from, T? to);
    protected abstract T? ConvertFromString(string? value);
    protected abstract string? ConvertToString(T? value);

    protected abstract T Create(int year, int month, int day);
    protected abstract T AddMonths(T date, int i);

    protected virtual TPeriod ConvertFromStringPeriod(string? value)
    {
        if (!string.IsNullOrWhiteSpace(value))
        {
            Match m = _regex.Match(value);
            if (m.Success)
            {
                T? from = ConvertFromString(m.Groups[1].Value);
                T? to = ConvertFromString(m.Groups[2].Value);
                return CreatePeriod(from, to);
            }
        }

        throw new ArgumentException($"'{value}' is not a valid period string");
    }

    protected virtual string? ConvertToString(TPeriod? period)
        => ConvertToString(period as IPeriod<T>);

    protected virtual string? ConvertToString(IPeriod<T>? period)
        => period is null ? null : $"[{ConvertToString(period.From)},{ConvertToString(period.To)}[";

    // _ => empty
    // X => part of interval (period = 1 month)
    // . => minus or plus infinity (depending on first or last character)
    //
    // "___XXXX__XXXXX_XXXXX_XXXXX_XXXX."
    // "___X__X__XXXXX_XXXXX_XXXXX_XXXX_"
    public TPeriod[] GeneratePeriods(T startDate, string? intervalString)
    {
        if (intervalString is null)
        {
            return [];
        }

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
                    throw new InternalProgrammingError("Invalid state string!");
            }

            currentDate = AddMonths(currentDate, 1);
        }

        return periods.ToArray();
    }
}
