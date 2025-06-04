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

using System.Text;
using System.Text.Json.Serialization;

namespace PPWCode.Util.Validation.IV
{
    public abstract class AbstractIdentification
        : IIdentification,
          IEquatable<AbstractIdentification>
    {
        [JsonIgnore]
        private string? _cleanedVersion;

        [JsonIgnore]
        private string? _cleanedVersionWithoutPadding;

        [JsonIgnore]
        private string? _electronicVersion;

        [JsonIgnore]
        private bool? _isStrictValid;

        [JsonIgnore]
        private bool? _isValid;

        [JsonIgnore]
        private string? _paperVersion;

        protected AbstractIdentification(string? rawVersion)
        {
            RawVersion = rawVersion;
        }

        protected abstract string OnPaperVersion { get; }

        public abstract char PaddingCharacter { get; }

        protected virtual string OnElectronicVersion
            => CleanedVersion;

        protected virtual string OnCleanedVersionWithoutPadding
            => GetValidStream(RawVersion);

        public bool Equals(AbstractIdentification? other)
        {
            if (ReferenceEquals(null, other))
            {
                return false;
            }

            if (ReferenceEquals(this, other))
            {
                return true;
            }

            return string.Equals(CleanedVersion, other.CleanedVersion);
        }

        [JsonIgnore]
        public string CleanedVersionWithoutPadding
            => _cleanedVersionWithoutPadding ??= OnCleanedVersionWithoutPadding;

        [JsonIgnore]
        public string CleanedVersion
            => _cleanedVersion ??= Pad(CleanedVersionWithoutPadding);

        [JsonIgnore]
        public string? ElectronicVersion
        {
            get
            {
                _electronicVersion ??= IsValid ? OnElectronicVersion : string.Empty;
                return IsValid ? _electronicVersion : null;
            }
        }

        [JsonIgnore]
        public bool IsStrictValid
            => _isStrictValid ?? (bool)(_isStrictValid = Validate(RawVersion));

        [JsonIgnore]
        public bool IsValid
            => _isValid ?? (bool)(_isValid = Validate(CleanedVersion));

        [JsonIgnore]
        public string? PaperVersion
        {
            get
            {
                _paperVersion ??= IsValid ? OnPaperVersion : string.Empty;
                return IsValid ? _paperVersion : null;
            }
        }

        [JsonInclude]
        public string? RawVersion { get; init; }

        [JsonIgnore]
        public virtual int StandardMaxLength
            => StandardMinLength;

        public abstract int StandardMinLength { get; }

        protected abstract bool OnValidate(string identification);

        protected virtual bool Validate(string? identification)
        {
            if ((identification != null)
                && (StandardMinLength <= identification.Length)
                && (identification.Length <= StandardMaxLength)
                && identification.All(IsValidChar))
            {
                return OnValidate(identification);
            }

            return false;
        }

        protected virtual string GetValidStream(string? stream)
        {
            if (stream == null)
            {
                return string.Empty;
            }

            StringBuilder sb = new (stream.Length);
            foreach (char ch in stream.Where(IsValidChar))
            {
                sb.Append(ch);
            }

            return sb.ToString();
        }

        protected virtual bool IsValidChar(char ch)
            => char.IsDigit(ch);

        protected virtual string Pad(string identification)
            => identification.PadLeft(StandardMaxLength, PaddingCharacter);

        public override bool Equals(object? obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }

            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            if (obj.GetType() != GetType())
            {
                return false;
            }

            return Equals((AbstractIdentification)obj);
        }

        public override int GetHashCode()
            => CleanedVersion.GetHashCode();

        public override string? ToString()
            => IsValid ? PaperVersion : RawVersion;

        public static bool operator ==(AbstractIdentification? left, AbstractIdentification? right)
            => Equals(left, right);

        public static bool operator !=(AbstractIdentification? left, AbstractIdentification? right)
            => !Equals(left, right);
    }
}
