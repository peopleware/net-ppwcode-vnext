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
using System.Text;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;

namespace PPWCode.Util.Validation.IV
{
    /// <summary>
    ///     see <see href="https://en.wikipedia.org/wiki/ISO_9362" />
    /// </summary>
    public class BIC : AbstractIdentification
    {
        /// <summary>
        ///     see <see href="https://en.wikipedia.org/wiki/ISO_3166-1_alpha-2" />
        /// </summary>
        public static readonly IList<string> CountryCodeList =
            new List<string>
            {
                "AD",
                "AE",
                "AF",
                "AG",
                "AI",
                "AL",
                "AM",
                "AO",
                "AQ",
                "AR",
                "AS",
                "AT",
                "AU",
                "AW",
                "AX",
                "AZ",
                "BA",
                "BB",
                "BD",
                "BE",
                "BF",
                "BG",
                "BH",
                "BI",
                "BJ",
                "BL",
                "BM",
                "BN",
                "BO",
                "BQ",
                "BR",
                "BS",
                "BT",
                "BV",
                "BW",
                "BY",
                "BZ",
                "CA",
                "CC",
                "CD",
                "CF",
                "CG",
                "CH",
                "CI",
                "CK",
                "CL",
                "CM",
                "CN",
                "CO",
                "CR",
                "CU",
                "CV",
                "CW",
                "CX",
                "CY",
                "CZ",
                "DE",
                "DJ",
                "DK",
                "DM",
                "DO",
                "DZ",
                "EC",
                "EE",
                "EG",
                "EH",
                "ER",
                "ES",
                "ET",
                "FI",
                "FJ",
                "FK",
                "FM",
                "FO",
                "FR",
                "GA",
                "GB",
                "GD",
                "GE",
                "GF",
                "GG",
                "GH",
                "GI",
                "GL",
                "GM",
                "GN",
                "GP",
                "GQ",
                "GR",
                "GS",
                "GT",
                "GU",
                "GW",
                "GY",
                "HK",
                "HM",
                "HN",
                "HR",
                "HT",
                "HU",
                "ID",
                "IE",
                "IL",
                "IM",
                "IN",
                "IO",
                "IQ",
                "IR",
                "IS",
                "IT",
                "JE",
                "JM",
                "JO",
                "JP",
                "KE",
                "KG",
                "KH",
                "KI",
                "KM",
                "KN",
                "KP",
                "KR",
                "KW",
                "KY",
                "KZ",
                "LA",
                "LB",
                "LC",
                "LI",
                "LK",
                "LR",
                "LS",
                "LT",
                "LU",
                "LV",
                "LY",
                "MA",
                "MC",
                "MD",
                "ME",
                "MF",
                "MG",
                "MH",
                "MK",
                "ML",
                "MM",
                "MN",
                "MO",
                "MP",
                "MQ",
                "MR",
                "MS",
                "MT",
                "MU",
                "MV",
                "MW",
                "MX",
                "MY",
                "MZ",
                "NA",
                "NC",
                "NE",
                "NF",
                "NG",
                "NI",
                "NL",
                "NO",
                "NP",
                "NR",
                "NU",
                "NZ",
                "OM",
                "PA",
                "PE",
                "PF",
                "PG",
                "PH",
                "PK",
                "PL",
                "PM",
                "PN",
                "PR",
                "PS",
                "PT",
                "PW",
                "PY",
                "QA",
                "RE",
                "RO",
                "RS",
                "RU",
                "RW",
                "SA",
                "SB",
                "SC",
                "SD",
                "SE",
                "SG",
                "SH",
                "SI",
                "SJ",
                "SK",
                "SL",
                "SM",
                "SN",
                "SO",
                "SR",
                "SS",
                "ST",
                "SV",
                "SX",
                "SY",
                "SZ",
                "TC",
                "TD",
                "TF",
                "TG",
                "TH",
                "TJ",
                "TK",
                "TL",
                "TM",
                "TN",
                "TO",
                "TR",
                "TT",
                "TV",
                "TW",
                "TZ",
                "UA",
                "UG",
                "UM",
                "US",
                "UY",
                "UZ",
                "VA",
                "VC",
                "VE",
                "VG",
                "VI",
                "VN",
                "VU",
                "WF",
                "WS",
                "YE",
                "YT",
                "ZA",
                "ZM",
                "ZW"
            };

        // ReSharper disable once InconsistentNaming
        private static readonly Regex BICFormatRegex =
            new ("^([A-Za-z]{4})([A-Za-z]{2})([A-Za-z0-9]{2})([A-Za-z0-9]{3})?$", RegexOptions.Compiled | RegexOptions.CultureInvariant);

        public BIC(string rawVersion)
            : base(rawVersion)
        {
        }

        protected override string OnCleanedVersionWithoutPadding
            => GetValidStream(RawVersion).ToUpper();

        protected override string OnPaperVersion
            => CleanedVersion;

        [JsonIgnore]
        [ExcludeFromCodeCoverage]
        public override char PaddingCharacter
            => throw new InvalidOperationException();

        [JsonIgnore]
        public override int StandardMaxLength
            => 11;

        [JsonIgnore]
        public override int StandardMinLength
            => 8;

        protected override string Pad(string identification)
            => identification;

        protected override bool IsValidChar(char ch)
            => char.IsLetterOrDigit(ch);

        protected override bool OnValidate(string identification)
        {
            StringBuilder sb = new (identification);

            // Check regular expression
            if (!BICFormatRegex.IsMatch(sb.ToString()))
            {
                return false;
            }

            // Check country code
            string countryCode = identification.Substring(4, 2).ToUpper();
            if (!CountryCodeList.Contains(countryCode))
            {
                return false;
            }

            return true;
        }

        public static implicit operator BIC(string identification)
            => new (identification);
    }
}
