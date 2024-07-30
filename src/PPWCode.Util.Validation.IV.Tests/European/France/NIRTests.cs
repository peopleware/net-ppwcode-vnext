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

using PPWCode.Util.Validation.IV.European.France;

namespace PPWCode.Util.Validation.IV.Tests.European.France
{
    [SuppressMessage("ReSharper", "InconsistentNaming", Justification = "Test")]
    [SuppressMessage("ReSharper", "UnusedMember.Local", Justification = "Test")]
    public class NIRTests : BaseTests
    {
        public static IEnumerable InvalidIdentifications
        {
            get
            {
                yield return null;
                yield return string.Empty;
                yield return "1";
                yield return "1234123";
            }
        }

        public static IEnumerable StrictValidIdentifications
        {
            get
            {
                yield return "151024610204372";
                yield return "278112B05000211";
            }
        }

        public static IEnumerable ValidIdentifications
        {
            get
            {
                foreach (object insee in StrictValidIdentifications)
                {
                    yield return insee;
                }

                yield return "FR 151024610204372";
                yield return "1 51 02 46102 043 72";
                yield return "1.51.02.46102.043-72";
            }
        }

        public static IEnumerable PaperVersions
        {
            get { yield return new TestCaseData("151024610204372").Returns("1 51 02 46102 043 72"); }
        }

        private static IEnumerable BirthDates
        {
            get { yield return new TestCaseData("151024610204372").Returns(new DateTime(1951, 2, 1)); }
        }

        private static IEnumerable Sexes
        {
            get { yield return new TestCaseData("151024610204372").Returns(Sex.MALE); }
        }

        [Test]
        [TestCaseSource(nameof(InvalidIdentifications))]
        public void bsn_is_not_valid(string identification)
        {
            // Arrange
            NIR nir = new (identification);

            // Act

            // Assert
            Assert.That(nir.IsValid, Is.False);
            Assert.That(nir.IsStrictValid, Is.False);
            Assert.That(nir.ElectronicVersion, Is.Null);
            Assert.That(nir.PaperVersion, Is.Null);
        }

        [Test]
        [TestCaseSource(nameof(StrictValidIdentifications))]
        public void bsn_is_strict_valid(string identification)
        {
            // Arrange
            NIR nir = new (identification);

            // Act

            // Assert
            Assert.That(nir.IsValid, Is.True);
            Assert.That(nir.IsStrictValid, Is.True);
            Assert.That(nir.ElectronicVersion, Is.EqualTo(nir.CleanedVersion));
            Assert.That(nir.ElectronicVersion, Is.EqualTo(nir.RawVersion));
            Assert.That(nir.PaperVersion, Is.Not.Null);
        }

        [Test]
        [TestCaseSource(nameof(ValidIdentifications))]
        public void bsn_is_valid(string identification)
        {
            // Arrange
            NIR nir = new (identification);

            // Act

            // Assert
            Assert.That(nir.IsValid, Is.True);
            Assert.That(nir.ElectronicVersion, Is.Not.Null);
            Assert.That(nir.PaperVersion, Is.Not.Null);
        }

        [Test]
        [TestCaseSource(nameof(ValidIdentifications))]
        public void check_binairy_serializable(string identification)
        {
            // Arrange
            NIR expected = new (identification);

            // Act
            NIR? actual = DeepCloneUsingBinaryFormatter(expected);

            // Assert
            Assert.That(actual, Is.Not.Null);
            Assert.That(actual!.RawVersion, Is.EqualTo(expected.RawVersion));
            Assert.That(actual.TwoIsoLetterCountryCode, Is.EqualTo(expected.TwoIsoLetterCountryCode));
        }

        [Test]
        [TestCaseSource(nameof(BirthDates))]
        public DateTime? check_birthdate(string identification)
        {
            // Arrange
            NIR nir = new (identification);

            // Act

            // Assert
            return nir.BirthDate;
        }

        [Test]
        [TestCaseSource(nameof(PaperVersions))]
        public string? check_paperversion(string identification)
        {
            // Arrange
            NIR nir = new (identification);

            // Act

            // Assert
            Assert.That(nir.IsValid, Is.True);
            Assert.That(nir.ElectronicVersion, Is.Not.Null);
            return nir.PaperVersion;
        }

        [Test]
        [TestCaseSource(nameof(Sexes))]
        public Sex? nir_sex(string identification)
        {
            // Arrange
            NIR nir = new (identification);

            // Act

            // Assert
            return nir.Sex;
        }
    }
}
