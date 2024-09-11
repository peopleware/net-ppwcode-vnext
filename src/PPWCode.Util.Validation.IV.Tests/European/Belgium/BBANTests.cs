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

using System.Collections;
using System.Diagnostics.CodeAnalysis;

using NUnit.Framework;

using PPWCode.Util.Validation.IV.European.Belgium;

namespace PPWCode.Util.Validation.IV.Tests.European.Belgium
{
    [SuppressMessage("ReSharper", "InconsistentNaming", Justification = "Test")]
    [SuppressMessage("ReSharper", "UnusedMember.Local", Justification = "Test")]
    public class BBANTests : BaseTests
    {
        public static IEnumerable InvalidIdentifications
        {
            get
            {
                yield return null;
                yield return string.Empty;
                yield return "1";
                yield return "12341234";
            }
        }

        public static IEnumerable StrictValidIdentifications
        {
            get { yield return "850895676978"; }
        }

        public static IEnumerable ValidIdentifications
        {
            get
            {
                foreach (object bban in StrictValidIdentifications)
                {
                    yield return bban;
                }

                yield return "850895676978 ";
                yield return "850-8956769-78";
                yield return "BE 850895676978";
                yield return "BE 850-89.56.76-978";
            }
        }

        public static IEnumerable PaperVersions
        {
            get
            {
                yield return new TestCaseData("850895676978").Returns("850-8956769-78");
                yield return new TestCaseData("BE 850-89.56.76-978").Returns("850-8956769-78");
            }
        }

        [Test]
        [TestCaseSource(nameof(ValidIdentifications))]
        public void bban_can_convert_valid_identifications_to_iban(string identification)
        {
            // Arrange
            BBAN bban = new (identification);
            Assert.That(bban.IsValid, Is.True);

            // Act
            IBAN? actual = bban.AsIBAN;

            // Assert
            Assert.That(actual, Is.Not.Null);
            Assert.That(actual!.IsValid, Is.True);
            Assert.That(actual.IsStrictValid, Is.True);
        }

        [Test]
        [TestCaseSource(nameof(InvalidIdentifications))]
        public void bban_can_not_convert_invalid_identifications_to_iban(string identification)
        {
            // Arrange
            BBAN bban = new (identification);
            Assert.That(bban.IsValid, Is.False);

            // Act
            IBAN? actual = bban.AsIBAN;

            // Assert
            Assert.That(actual, Is.Null);
        }

        [Test]
        [TestCaseSource(nameof(InvalidIdentifications))]
        public void bban_is_not_valid(string identification)
        {
            // Arrange
            BBAN bban = new (identification);

            // Act

            // Assert
            Assert.That(bban.IsValid, Is.False);
            Assert.That(bban.IsStrictValid, Is.False);
            Assert.That(bban.ElectronicVersion, Is.Null);
            Assert.That(bban.PaperVersion, Is.Null);
        }

        [Test]
        [TestCaseSource(nameof(StrictValidIdentifications))]
        public void bban_is_strict_valid(string identification)
        {
            // Arrange
            BBAN bban = new (identification);

            // Act

            // Assert
            Assert.That(bban.IsValid, Is.True);
            Assert.That(bban.IsStrictValid, Is.True);
            Assert.That(bban.ElectronicVersion, Is.EqualTo(bban.CleanedVersion));
            Assert.That(bban.ElectronicVersion, Is.EqualTo(bban.RawVersion));
            Assert.That(bban.PaperVersion, Is.Not.Null);
        }

        [Test]
        [TestCaseSource(nameof(ValidIdentifications))]
        public void bban_is_valid(string identification)
        {
            // Arrange
            BBAN bban = new (identification);

            // Act

            // Assert
            Assert.That(bban.IsValid, Is.True);
            Assert.That(bban.ElectronicVersion, Is.Not.Null);
            Assert.That(bban.PaperVersion, Is.Not.Null);
        }

        [Test]
        [TestCaseSource(nameof(ValidIdentifications))]
        public void check_binairy_serializable(string identification)
        {
            // Arrange
            BBAN expected = new (identification);

            // Act
            BBAN? actual = DeepClone(expected);

            // Assert
            Assert.That(actual, Is.Not.Null);
            Assert.That(actual!.RawVersion, Is.EqualTo(expected.RawVersion));
        }

        [Test]
        [TestCaseSource(nameof(PaperVersions))]
        public string? check_paperversion(string identification)
        {
            // Arrange
            BBAN bban = new (identification);

            // Act

            // Assert
            Assert.That(bban.IsValid, Is.True);
            Assert.That(bban.ElectronicVersion, Is.Not.Null);
            return bban.PaperVersion;
        }
    }
}
