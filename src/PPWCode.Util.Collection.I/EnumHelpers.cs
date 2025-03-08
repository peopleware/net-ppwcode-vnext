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
