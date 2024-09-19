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

public abstract class DateTimeOffsetPeriod
    : Period<DateTimeOffset>,
      IDateTimeOffsetPeriod
{
    // ReSharper disable once UnusedMember.Global
    protected DateTimeOffsetPeriod()
    {
    }

    protected DateTimeOffsetPeriod(DateTimeOffset? from, DateTimeOffset? to)
        : base(from, to)
    {
    }

    /// <inheritdoc />
    public override DateTimeOffset MinValue
        => DateTimeOffset.MinValue;

    /// <inheritdoc />
    public override DateTimeOffset MaxValue
        => DateTimeOffset.MaxValue;
}
