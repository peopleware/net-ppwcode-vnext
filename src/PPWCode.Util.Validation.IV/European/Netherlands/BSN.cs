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

using System.Globalization;
using System.Text.Json.Serialization;

namespace PPWCode.Util.Validation.IV.European.Netherlands
{
    /// <summary>
    ///     see <see href="https://nl.wikipedia.org/wiki/Burgerservicenummer" />.
    /// </summary>
    public class BSN
        : AbstractNlIdentification,
          INationalNumberIdentification
    {
        public BSN(string? rawVersion)
            : base(rawVersion)
        {
        }

        [JsonIgnore]
        public override char PaddingCharacter
            => '0';

        protected override string OnPaperVersion
            => CleanedVersion;

        [JsonIgnore]
        public override int StandardMinLength
            => 8;

        [JsonIgnore]
        public override int StandardMaxLength
            => 9;

        [JsonIgnore]
        public DateTime? BirthDate
            => null;

        [JsonIgnore]
        public Sex? Sex
            => null;

        protected override bool OnValidate(string identification)
        {
            if (identification.Length != StandardMaxLength)
            {
                return false;
            }

            if (identification == new string('0', StandardMaxLength))
            {
                return false;
            }

            long checkSum = 0L;
            for (int i = 0; i < StandardMaxLength; i++)
            {
                int n = 9 - i;
                int product = CharUnicodeInfo.GetDecimalDigitValue(identification[i]) * n;
                if (n == 1)
                {
                    product = -product;
                }

                checkSum += product;
            }

            long modulo11 = checkSum % 11;
            return modulo11 == 0;
        }

        public static implicit operator BSN(string identification)
            => new (identification);
    }
}
