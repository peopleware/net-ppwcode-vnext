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
    public class OGM : AbstractIdentification
    {
        public OGM(string? rawVersion)
            : base(rawVersion)
        {
        }

        /// <inheritdoc />
        protected override string OnPaperVersion
            => $"+++{CleanedVersion.Substring(0, 3)}/{CleanedVersion.Substring(3, 4)}/{CleanedVersion.Substring(7, 5)}+++";

        /// <inheritdoc />
        [JsonIgnore]
        public override char PaddingCharacter
            => '0';

        /// <inheritdoc />
        [JsonIgnore]
        public override int StandardMinLength
            => 12;

        /// <inheritdoc />
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

        public static implicit operator OGM(string identification)
            => new (identification);
    }
}
