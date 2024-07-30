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

using PPWCode.Util.Validation.IV.European.Belgium;

namespace PPWCode.Util.Validation.IV
{
    public class IBAN : AbstractIdentification
    {
        /// <summary>
        ///     see <see href="https://en.wikipedia.org/wiki/International_Bank_Account_Number" />
        /// </summary>
        public static readonly IDictionary<string, IbanCountry> IBANCountryInformation =
            new Dictionary<string, IbanCountry>
            {
                // Albania
                { "AL", new IbanCountry(28, "8n,16c") },

                // Andorra
                { "AD", new IbanCountry(24, "8n,12c") },

                // Austria
                { "AT", new IbanCountry(20, "16n") },

                // Azerbaijan
                { "AZ", new IbanCountry(28, "4c,20n") },

                // Bahrain
                { "BH", new IbanCountry(22, "4a,14c") },

                // Belarus
                { "BY", new IbanCountry(28, "4c,20n") },

                // Belgium
                { "BE", new IbanCountry(16, "12n") },

                // Bosnia and Herzegovina
                { "BA", new IbanCountry(20, "16n") },

                // Brazil
                { "BR", new IbanCountry(29, "23n,1a,1c") },

                // Bulgaria
                { "BG", new IbanCountry(22, "4a,6n,8c") },

                // Costa Rica
                { "CR", new IbanCountry(22, "18n") },

                // Croatia
                { "HR", new IbanCountry(21, "17n") },

                // Cyprus
                { "CY", new IbanCountry(28, "8n,16c") },

                // Czech Republic
                { "CZ", new IbanCountry(24, "20n") },

                // Denmark
                { "DK", new IbanCountry(18, "14n") },

                // Dominican Republic
                { "DO", new IbanCountry(28, "4a,20n") },

                // East Timor
                { "TL", new IbanCountry(23, "19n") },

                // Estonia
                { "EE", new IbanCountry(20, "16n") },

                // Faroe Islands
                { "FO", new IbanCountry(18, "14n") },

                // Finland
                { "FI", new IbanCountry(18, "14n") },

                // France
                { "FR", new IbanCountry(27, "10n,11c,2n") },

                // Georgia
                { "GE", new IbanCountry(22, "2c,16n") },

                // Germany
                { "DE", new IbanCountry(22, "18n") },

                // Gibraltar
                { "GI", new IbanCountry(23, "4a,15c") },

                // Greece
                { "GR", new IbanCountry(27, "7n,16c") },

                // Greenland
                { "GL", new IbanCountry(18, "14n") },

                // Guatemala
                { "GT", new IbanCountry(28, "4c,20c") },

                // Hungary
                { "HU", new IbanCountry(28, "24n") },

                // Iceland
                { "IS", new IbanCountry(26, "22n") },

                // Ireland
                { "IE", new IbanCountry(22, "4c,14n") },

                // Israel
                { "IL", new IbanCountry(23, "19n") },

                // Italy
                { "IT", new IbanCountry(27, "1a,10n,12c") },

                // Jordan
                { "JO", new IbanCountry(30, "4a,22n") },

                // Kazakhstan
                { "KZ", new IbanCountry(20, "3n,13c") },

                // Kosovo
                { "XK", new IbanCountry(20, "4n,10n,2n") },

                // Kuwait
                { "KW", new IbanCountry(30, "4a,22c") },

                // Latvia
                { "LV", new IbanCountry(21, "4a,13c") },

                // Lebanon
                { "LB", new IbanCountry(28, "4n,20c") },

                // Liechtenstein
                { "LI", new IbanCountry(21, "5n,12c") },

                // Lithuania
                { "LT", new IbanCountry(20, "16n") },

                // Luxembourg
                { "LU", new IbanCountry(20, "3n,13c") },

                // Macedonia
                { "MK", new IbanCountry(19, "3n,10c,2n") },

                // Malta
                { "MT", new IbanCountry(31, "4a,5n,18c") },

                // Mauritania
                { "MR", new IbanCountry(27, "23n") },

                // Mauritius
                { "MU", new IbanCountry(30, "4a,19n,3a") },

                // Monaco
                { "MC", new IbanCountry(27, "10n,11c,2n") },

                // Moldova
                { "MD", new IbanCountry(24, "2c,18c") },

                // Montenegro
                { "ME", new IbanCountry(22, "18n") },

                // Netherlands
                { "NL", new IbanCountry(18, "4a,10n") },

                // Norway
                { "NO", new IbanCountry(15, "11n") },

                // Pakistan
                { "PK", new IbanCountry(24, "4c,16n") },

                // Palestinian territories
                { "PS", new IbanCountry(29, "4c,21n") },

                // Poland
                { "PL", new IbanCountry(28, "24n") },

                // Portugal
                { "PT", new IbanCountry(25, "21n") },

                // Qatar
                { "QA", new IbanCountry(29, "4a,21c") },

                // Romania
                { "RO", new IbanCountry(24, "4a,16c") },

                // San Marino
                { "SM", new IbanCountry(27, "1a,10n,12c") },

                // Saudi Arabia
                { "SA", new IbanCountry(24, "2n,18c") },

                // Serbia
                { "RS", new IbanCountry(22, "18n") },

                // Slovakia
                { "SK", new IbanCountry(24, "20n") },

                // Slovenia
                { "SI", new IbanCountry(19, "15n") },

                // Spain
                { "ES", new IbanCountry(24, "20n") },

                // Sweden
                { "SE", new IbanCountry(24, "20n") },

                // Switzerland
                { "CH", new IbanCountry(21, "5n,12c") },

                // Tunisia
                { "TN", new IbanCountry(24, "20n") },

                // Turkey
                { "TR", new IbanCountry(26, "5n,17c") },

                // United Arab Emirates
                { "AE", new IbanCountry(23, "3n,16n") },

                // United Kingdom
                { "GB", new IbanCountry(22, "4a,14n") },

                // Virgin Islands British
                { "VG", new IbanCountry(24, "4c,16n") }
            };

        // ReSharper disable once InconsistentNaming
        private static readonly Regex IBANFormatRegex =
            new ("^(?<n>\\d+)(?<t>(n|a|c))$", RegexOptions.Compiled | RegexOptions.CultureInvariant);

        public static readonly IDictionary<char, int> LetterConversions =
            new Dictionary<char, int>
            {
                { 'A', 10 },
                { 'B', 11 },
                { 'C', 12 },
                { 'D', 13 },
                { 'E', 14 },
                { 'F', 15 },
                { 'G', 16 },
                { 'H', 17 },
                { 'I', 18 },
                { 'J', 19 },
                { 'K', 20 },
                { 'L', 21 },
                { 'M', 22 },
                { 'N', 23 },
                { 'O', 24 },
                { 'P', 25 },
                { 'Q', 26 },
                { 'R', 27 },
                { 'S', 28 },
                { 'T', 29 },
                { 'U', 30 },
                { 'V', 31 },
                { 'W', 32 },
                { 'X', 33 },
                { 'Y', 34 },
                { 'Z', 35 }
            };

        private string? _twoLetterIsoLanguageName;

        public IBAN(string? rawVersion)
            : base(rawVersion)
        {
        }

        protected override string OnPaperVersion
        {
            get
            {
                StringBuilder sb = new (50);
                for (int i = 0, j = 0; i < CleanedVersion.Length; i++)
                {
                    char ch = CleanedVersion[i];
                    if (j == 4)
                    {
                        sb.Append(' ');
                        j = 0;
                    }

                    j++;
                    sb.Append(ch);
                }

                return sb.ToString();
            }
        }

        [JsonIgnore]
        [ExcludeFromCodeCoverage]
        public override char PaddingCharacter
            => throw new InvalidOperationException();

        [JsonIgnore]
        public override int StandardMaxLength
            => 34;

        [JsonIgnore]
        public override int StandardMinLength
            => 14;

        [JsonIgnore]
        public BBAN? AsBBAN
            => IsValid && string.Equals(Country, "BE", StringComparison.InvariantCulture)
                   ? new BBAN(CleanedVersion.Substring(4, 12))
                   : null;

        [JsonIgnore]
        public string Country
            => _twoLetterIsoLanguageName ??= CleanedVersion.Substring(0, 2);

        protected override string Pad(string identification)
            => identification;

        protected override bool IsValidChar(char ch)
            => char.IsLetterOrDigit(ch);

        protected override bool OnValidate(string identification)
        {
            // phase 1. Check country code
            string countryCode = identification.Substring(0, 2).ToUpper();
            if (!IBANCountryInformation.TryGetValue(countryCode, out IbanCountry ibanCountry))
            {
                return false;
            }

            StringBuilder sb = new (identification);

            // phase 2: Check the length of the IBAN nr according the country code
            if (ibanCountry.IBANLength != sb.Length)
            {
                return false;
            }

            // phase 3: Check regular expression if known
            if (ibanCountry.Pattern != string.Empty)
            {
                Regex regex = new (string.Concat("^", countryCode, @"\d{2}", ConvertToRegEx(ibanCountry.Pattern), "$"));
                if (!regex.IsMatch(sb.ToString()))
                {
                    return false;
                }
            }

            // phase 4: Move the first four characters of the IBAN to the right of the number.
            sb.Append(sb.ToString().Substring(0, 4));
            sb.Remove(0, 4);

            string tmp = sb.ToString();

            // phase 5: Convert the letters into numerics in accordance with the conversion table.
            StringBuilder sb2 = new (50);
            foreach (char ch in tmp)
            {
                if (char.IsLetter(ch))
                {
                    if (LetterConversions.TryGetValue(ch, out int num))
                    {
                        sb2.Append(num.ToString());
                    }
                }
                else
                {
                    sb2.Append(ch);
                }
            }

            // phase 5: Check MOD 97-10 (see ISO 7064).
            // For the check digits to be correct,
            // the remainder after calculating the modulus 97 must be 1.
            // We will use integers instead of floating point numbers for precision.
            // BUT if the number is too long for the software implementation of
            // integers (a signed 32/64 bits represents 9/18 digits), then the
            // calculation can be split up into consecutive remainder calculations
            // on integers with a maximum of 9 or 18 digits.
            // I wil choose 32-bit integers.
            int mod97 = 0, n = 9;
            string s9 = sb2.ToString().Substring(0, n);
            while (s9.Length > 0)
            {
                sb2.Remove(0, n);
                mod97 = int.Parse(s9) % 97;
                if (sb2.Length > 0)
                {
                    n = mod97 < 10 ? 8 : 7;
                    n = sb2.Length < n ? sb2.Length : n;
                    s9 = string.Concat(mod97.ToString(), sb2.ToString().Substring(0, n));
                }
                else
                {
                    s9 = string.Empty;
                }
            }

            return mod97 == 1;
        }

        private string ConvertToRegEx(string pattern)
        {
            StringBuilder sb = new ();
            string[] items =
                pattern
                    .Split(
                        new[]
                        {
                            ','
                        },
                        StringSplitOptions.RemoveEmptyEntries);
            foreach (string item in items)
            {
                Match match = IBANFormatRegex.Match(item);
                if (match.Success)
                {
                    string t = match.Groups["t"].Value;
                    string n = match.Groups["n"].Value;
                    switch (t)
                    {
                        case "n":
                            sb.AppendFormat(@"\d{{{0}}}", n);
                            break;
                        case "a":
                            sb.AppendFormat(@"[A-Z]{{{0}}}", n);
                            break;
                        case "c":
                            sb.AppendFormat(@"[0-9A-Za-z]{{{0}}}", n);
                            break;
                    }
                }
            }

            return sb.ToString();
        }

        public struct IbanCountry
        {
            public IbanCountry(int ibanLength, string pattern)
            {
                IBANLength = ibanLength;
                Pattern = pattern;
            }

            public int IBANLength { get; }

            public string Pattern { get; }
        }
    }
}
