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
using System.Text.RegularExpressions;

namespace PPWCode.Util.Validation.IV.European.Belgium
{
    public class DMFA : AbstractBeIdentification
    {
        /// <summary>
        ///     see
        ///     <see
        ///         href="https://www.ksz-bcss.fgov.be/sites/default/files/assets/diensten_en_support/documentatie/dmfa_stromen_xml.pdf" />
        /// </summary>
        private static readonly Regex _dmfaRegex =
            new ("^(DMFA|DPPL)(T|A|P)\\d{9}(\\d|[A-Z])$", RegexOptions.Compiled | RegexOptions.CultureInvariant);

        public DMFA(string rawVersion)
            : base(rawVersion)
        {
        }

        [JsonIgnore]
        public virtual long? AsNumber
            => (RawVersion != null) && _dmfaRegex.IsMatch(RawVersion)
                   ? long.Parse(RawVersion.Substring(5, 9))
                   : null;

        protected override string OnPaperVersion
            => CleanedVersion;

        [JsonIgnore]
        [ExcludeFromCodeCoverage]
        public override char PaddingCharacter
            => throw new InvalidOperationException();

        [JsonIgnore]
        public override int StandardMinLength
            => 15;

        protected override string Pad(string identification)
            => identification;

        protected override bool IsValidChar(char ch)
            => char.IsLetterOrDigit(ch);

        protected override bool OnValidate(string identification)
            => AsNumber != null;
    }
}
