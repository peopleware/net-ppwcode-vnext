using PPWCode.Vernacular.Exceptions.V;

namespace PPWCode.Util.Collection.I;

public static class EnumHelpers
{
    public static T Parse<T>(this string value, bool ignoreCase = true)
        where T : struct
        => (T)Enum.Parse(typeof(T), value, ignoreCase);

    public static T Parse<T>(this int value)
        where T : struct
    {
        if (Enum.IsDefined(typeof(T), value))
        {
            return (T)Enum.ToObject(typeof(T), value);
        }

        throw new ProgrammingError($"{typeof(T).FullName} is not a defined enumeration type.");
    }

    public static T? TryParse<T>(this string? value, bool ignoreCase = true, T? fallbackValue = null)
        where T : struct
    {
        if (typeof(T).IsEnum)
        {
            return
                value is not null
                    ? Enum.TryParse(value, ignoreCase, out T @enum) ? @enum : fallbackValue
                    : fallbackValue;
        }

        throw new ProgrammingError($"{typeof(T).FullName} is not a defined enumeration type.");
    }

    public static T? TryParse<T>(this string value, T? fallbackValue)
        where T : struct
        => value.TryParse(true, fallbackValue);

    public static T? TryParse<T>(this int value, T? fallbackValue = null)
        where T : struct
        => Enum.IsDefined(typeof(T), value) ? (T?)Enum.ToObject(typeof(T), value) : fallbackValue;

    public static IEnumerable<T> GetValues<T>()
        where T : struct
        => Enum.GetValues(typeof(T)).Cast<T>();
}
