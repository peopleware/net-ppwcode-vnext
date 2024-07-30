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
using System.Text;
using System.Text.Json.Serialization;

namespace PPWCode.Util.Validation.IV.European.Belgium
{
    public class BBAN : AbstractBeIdentification
    {
        public BBAN(string? rawVersion)
            : base(rawVersion)
        {
        }

        protected override string OnPaperVersion
            => $"{CleanedVersion.Substring(0, 3)}-{CleanedVersion.Substring(3, 7)}-{CleanedVersion.Substring(10, 2)}";

        [JsonIgnore]
        public override char PaddingCharacter
            => '0';

        [JsonIgnore]
        public override int StandardMinLength
            => 12;

        [JsonIgnore]
        public IBAN? AsIBAN
        {
            get
            {
                if (IsValid)
                {
                    StringBuilder sb = new (CleanedVersion);

                    // Move BE + '00' at the end
                    sb.Append(IBAN.LetterConversions['B'].ToString(CultureInfo.InvariantCulture));
                    sb.Append(IBAN.LetterConversions['E'].ToString(CultureInfo.InvariantCulture));
                    sb.Append("00");

                    // Calculate Check Digits
                    // Calculate MOD 97-10 (see ISO 7064)
                    // For the check digits to be correct,
                    // the remainder after calculating the modulus 97 must be 1.
                    // We will use integers instead of floating point numbers for precision.
                    // BUT if the number is too long for the software implementation of
                    // integers (a signed 32/64 bits represents 9/18 digits), then the
                    // calculation can be split up into consecutive remainder calculations
                    // on integers with a maximum of 9 or 18 digits.
                    // I will choose 32-bit integers.
                    int mod97 = 0, n = 9;
                    string s9 = sb.ToString().Substring(0, n);
                    while (s9.Length > 0)
                    {
                        sb.Remove(0, n);
                        mod97 = int.Parse(s9) % 97;
                        if (sb.Length > 0)
                        {
                            n = mod97 < 10 ? 8 : 7;
                            n = sb.Length < n ? sb.Length : n;
                            s9 = string.Concat(mod97.ToString(CultureInfo.InvariantCulture), sb.ToString().Substring(0, n));
                        }
                        else
                        {
                            s9 = string.Empty;
                        }
                    }

                    mod97 = 98 - mod97;

                    return new IBAN(string.Concat("BE", mod97.ToString("00"), CleanedVersion));
                }

                return null;
            }
        }

        protected override bool OnValidate(string identification)
        {
            long rest = Mod97CheckNumber(long.Parse(identification.Substring(0, 10)));
            return rest == long.Parse(identification.Substring(10, 2));
        }

        private long Mod97CheckNumber(long baseNum)
        {
            long result = baseNum % 97;
            return result == 0 ? 97 : result;
        }
    }
}
