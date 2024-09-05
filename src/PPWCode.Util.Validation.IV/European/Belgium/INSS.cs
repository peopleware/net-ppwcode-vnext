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

using System.Text.Json.Serialization;

namespace PPWCode.Util.Validation.IV.European.Belgium
{
    public class INSS
        : AbstractBeIdentification,
          INationalNumberIdentification
    {
        private ParseResult? _parseResult;

        public INSS(string? rawVersion)
            : base(rawVersion)
        {
        }

        protected override string OnPaperVersion
            => $"{CleanedVersion.Substring(0, 2)}.{CleanedVersion.Substring(2, 2)}.{CleanedVersion.Substring(4, 2)}-{CleanedVersion.Substring(6, 3)}.{CleanedVersion.Substring(9, 2)}";

        [JsonIgnore]
        public override char PaddingCharacter
            => '0';

        /// <summary>
        ///     see <see href="https://www.ksz-bcss.fgov.be/nl/diensten-en-support/diensten/ksz-registers" /> for more information
        ///     about bis-numbers.
        /// </summary>
        [JsonIgnore]
        public bool? IsBisNumber
        {
            get
            {
                if (IsValid)
                {
                    int mm = int.Parse(CleanedVersion.Substring(2, 2));
                    return mm > 12;
                }

                return null;
            }
        }

        [JsonIgnore]
        public bool? IsNationalNumber
            => !IsBisNumber;

        [JsonIgnore]
        public Sex? Sex
        {
            get
            {
                _parseResult ??= ParseINSS();
                return _parseResult.Sex;
            }
        }

        [JsonIgnore]
        public override int StandardMinLength
            => 11;

        [JsonIgnore]
        public DateTime? BirthDate
        {
            get
            {
                _parseResult ??= ParseINSS();
                return _parseResult.BirthDate;
            }
        }

        protected override bool OnValidate(string identification)
            => ValidBefore2000(identification) || ValidAfter2000(identification);

        private ParseResult ParseINSS()
        {
            DateTime? birthdate = null;
            Sex? sex = null;

            if (IsValid)
            {
                bool calcSex = false;
                bool calcBirthDate = false;
                int yy = int.Parse(CleanedVersion.Substring(0, 2));
                int mm = int.Parse(CleanedVersion.Substring(2, 2));
                int dd = int.Parse(CleanedVersion.Substring(4, 2));
                int vvv = int.Parse(CleanedVersion.Substring(6, 3));

                int yyOffset;
                {
                    yyOffset = ValidBefore2000(CleanedVersion) ? 1900 : 2000;
                }

                yy += yyOffset;

                if (mm < 20)
                {
                    calcSex = true;
                    calcBirthDate = true;
                }
                else if (mm < 40)
                {
                    mm -= 20;
                    calcBirthDate = true;
                }
                else if (mm < 60)
                {
                    mm -= 40;
                    calcSex = true;
                    calcBirthDate = true;
                }

                if (calcSex)
                {
                    sex = vvv is 0 or 999
                              ? IV.Sex.NOT_KNOWN
                              : vvv % 2 == 1
                                  ? IV.Sex.MALE
                                  : IV.Sex.FEMALE;
                }

                if (calcBirthDate && mm is > 0 and < 13 && dd is > 0 and < 32)
                {
                    try
                    {
                        birthdate = new DateTime(yy, mm, dd);
                        if (birthdate > DateTime.Today)
                        {
                            birthdate = null;
                        }
                    }
                    catch
                    {
                        birthdate = null;
                    }
                }
            }

            return new ParseResult(birthdate, sex);
        }

        protected bool ValidBefore2000(string identification)
        {
            string number = identification.Substring(0, 9);
            long numberBefore2000 = long.Parse(number);
            int rest = 97 - int.Parse(identification.Substring(9, 2));
            return numberBefore2000 % 97 == rest;
        }

        protected bool ValidAfter2000(string identification)
        {
            string number = identification.Substring(0, 9);
            long numberAfter2000 = long.Parse(string.Concat('2', number));
            int rest = 97 - int.Parse(identification.Substring(9, 2));
            return numberAfter2000 % 97 == rest;
        }

        private class ParseResult
        {
            public ParseResult(DateTime? birthDate, Sex? sex)
            {
                BirthDate = birthDate;
                Sex = sex;
            }

            public DateTime? BirthDate { get; }

            public Sex? Sex { get; }
        }
    }
}
