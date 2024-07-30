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

using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace PPWCode.Util.Validation.IV.European.Belgium
{
    public class CompanyLocalUnitNumber : KBO
    {
        private static readonly char[] _validFirstChars = { '2', '3', '4', '5', '6', '7', '8' };

        private static readonly ISet<CompanyLocalUnitNumber> _validFictiveNumbers =
            new HashSet<CompanyLocalUnitNumber>
            {
                new ("8999999993"),
                new ("8999999104"),
                new ("8999999203"),
                new ("8999999302"),
                new ("8999999401"),
                new ("8999999005"),
                new ("8999999894")
            };

        /// <summary>
        ///     See
        ///     <see href="http://www.ejustice.just.fgov.be/cgi_loi/change_lg.pl?language=nl&la=N&cn=2003062432&table_name=wet" />
        /// </summary>
        /// <param name="rawVersion">The raw local unit number.</param>
        public CompanyLocalUnitNumber(string? rawVersion)
            : base(rawVersion)
        {
        }

        public static IEnumerable<CompanyLocalUnitNumber> ValidFictiveNumbers
            => _validFictiveNumbers;

        protected override string OnPaperVersion
            => $"{CleanedVersion.Substring(0, 1)}.{CleanedVersion.Substring(1, 3)}.{CleanedVersion.Substring(4, 3)}.{CleanedVersion.Substring(7, 3)}";

        [JsonIgnore]
        [ExcludeFromCodeCoverage]
        public override char PaddingCharacter
            => throw new InvalidOperationException();

        public bool IsFictiveNumber(string identification)
            => _validFictiveNumbers.Contains(new CompanyLocalUnitNumber(identification));

        protected override string Pad(string identification)
            => identification;

        protected override bool IsValidFirstChar(char ch)
            => _validFirstChars.Contains(ch);
    }
}
