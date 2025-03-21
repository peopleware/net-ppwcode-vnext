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

using PPWCode.Vernacular.Semantics.V;

namespace PPWCode.Util.Time.I;

public interface IPeriod<T>
    : ICivilizedObject
    where T : struct, IComparable<T>, IEquatable<T>
{
    T? From { get; }
    T? To { get; }

    T CoalesceFrom { get; }
    T CoalesceTo { get; }

    bool Contains(T other);
    bool Contains(IPeriod<T> other);
    bool Overlaps(IPeriod<T> other);
    bool IsCompletelyContainedWithin(IPeriod<T> other);
    IPeriod<T> OverlappingPeriod(IPeriod<T> other);
    T[] PointsInTime { get; }
}
