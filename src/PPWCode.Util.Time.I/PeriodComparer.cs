// Copyright 2024 by PeopleWare n.v..
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// http://www.apache.org/licenses/LICENSE-2.0
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

namespace PPWCode.Util.Time.I;

public class PeriodComparer<T> : IEqualityComparer<IPeriod<T>>
    where T : struct, IComparable<T>, IEquatable<T>
{
    public bool Equals(IPeriod<T>? x, IPeriod<T>? y)
    {
        if (ReferenceEquals(x, y))
        {
            return true;
        }

        if (x is null || y is null)
        {
            return false;
        }

        return x.From.Equals(y.From) && x.To.Equals(y.To);
    }

    /// <inheritdoc />
    public int GetHashCode(IPeriod<T> obj)
    {
        unchecked
        {
            return (obj.From.GetHashCode() * 397) ^ obj.To.GetHashCode();
        }
    }
}
