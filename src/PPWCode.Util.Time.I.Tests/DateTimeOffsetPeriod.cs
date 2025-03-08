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

namespace PPWCode.Util.Time.I.Tests;

public class DateTimeOffsetPeriod : I.DateTimeOffsetPeriod
{
    public DateTimeOffsetPeriod(DateTimeOffset? from, DateTimeOffset? to)
        : base(from, to)
    {
    }

    /// <inheritdoc />
    protected override SemanticException CreateInvalidExceptionFor(DateTimeOffset? from, DateTimeOffset? to)
        => new ("ERROR_PERIOD_FROM_MUST_BE_STRICTLY_BEFORE_TO");

    /// <inheritdoc />
    protected override IPeriod<DateTimeOffset> Create(DateTimeOffset? from, DateTimeOffset? to)
        => new DateTimeOffsetPeriod(from, to);
}
