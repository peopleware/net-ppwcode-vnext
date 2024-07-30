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
    public class INSSTests : BaseTests
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
            get
            {
                yield return "55250200801";
                yield return "01410191175";
                yield return "00030610329";
            }
        }

        public static IEnumerable ValidIdentifications
        {
            get
            {
                foreach (object inss in StrictValidIdentifications)
                {
                    yield return inss;
                }

                yield return "1410191175";
                yield return "30610329";
                yield return "BE 55250200801";
                yield return "BE 55.25.02 008-01";
            }
        }

        public static IEnumerable PaperVersions
        {
            get
            {
                yield return new TestCaseData("55250200801").Returns("55.25.02-008.01");
                yield return new TestCaseData("BE 55250200801").Returns("55.25.02-008.01");
            }
        }

        private static IEnumerable BisNumbers
        {
            get
            {
                foreach (object inss in InvalidIdentifications)
                {
                    yield return new TestCaseData(inss).Returns(null);
                }

                yield return new TestCaseData("08082207107").Returns(false);
                yield return new TestCaseData("56252201101").Returns(true);
                yield return new TestCaseData("01410191175").Returns(true);
            }
        }

        private static IEnumerable BirthDates
        {
            get
            {
                yield return new TestCaseData("06102614576").Returns(new DateTime(2006, 10, 26));
                yield return new TestCaseData("08290801251").Returns(new DateTime(2008, 09, 08));
                yield return new TestCaseData("08511500896").Returns(new DateTime(2008, 11, 15));
                yield return new TestCaseData("09051132455").Returns(new DateTime(1909, 05, 11));
                yield return new TestCaseData("24210700548").Returns(new DateTime(1924, 01, 07));
                yield return new TestCaseData("09501001134").Returns(new DateTime(1909, 10, 10));

                yield return new TestCaseData("00081503486").Returns(new DateTime(2000, 8, 15));
            }
        }

        private static IEnumerable Sexes
        {
            get
            {
                yield return new TestCaseData("06102614576").Returns(Sex.MALE);
                yield return new TestCaseData("08290801251").Returns(null);
                yield return new TestCaseData("09051132455").Returns(Sex.FEMALE);
            }
        }

        [Test]
        [TestCaseSource(nameof(ValidIdentifications))]
        public void check_binairy_serializable(string identification)
        {
            // Arrange
            INSS expected = new (identification);

            // Act
            INSS? actual = DeepCloneUsingBinaryFormatter(expected);

            // Assert
            Assert.That(actual, Is.Not.Null);
            Assert.That(actual!.RawVersion, Is.EqualTo(expected.RawVersion));
        }

        [Test]
        [TestCaseSource(nameof(BirthDates))]
        public DateTime? check_birthdate(string identification)
        {
            // Arrange
            INSS inss = new (identification);

            // Act

            // Assert
            return inss.BirthDate;
        }

        [Test]
        [TestCaseSource(nameof(BisNumbers))]
        public bool? check_bis_numbers(string identification)
        {
            // Arrange
            INSS inss = new (identification);

            // Act

            // Assert
            return inss.IsBisNumber;
        }

        [Test]
        [TestCaseSource(nameof(PaperVersions))]
        public string? check_paperversion(string identification)
        {
            // Arrange
            INSS inss = new (identification);

            // Act

            // Assert
            Assert.That(inss.IsValid, Is.True);
            Assert.That(inss.ElectronicVersion, Is.Not.Null);
            return inss.PaperVersion;
        }

        [Test]
        [TestCaseSource(nameof(InvalidIdentifications))]
        public void inss_is_not_valid(string identification)
        {
            // Arrange
            INSS inss = new (identification);

            // Act

            // Assert
            Assert.That(inss.IsValid, Is.False);
            Assert.That(inss.IsStrictValid, Is.False);
            Assert.That(inss.ElectronicVersion, Is.Null);
            Assert.That(inss.PaperVersion, Is.Null);
        }

        [Test]
        [TestCaseSource(nameof(StrictValidIdentifications))]
        public void inss_is_strict_valid(string identification)
        {
            // Arrange
            INSS inss = new (identification);

            // Act

            // Assert
            Assert.That(inss.IsValid, Is.True);
            Assert.That(inss.IsStrictValid, Is.True);
            Assert.That(inss.ElectronicVersion, Is.EqualTo(inss.CleanedVersion));
            Assert.That(inss.ElectronicVersion, Is.EqualTo(inss.RawVersion));
            Assert.That(inss.PaperVersion, Is.Not.Null);
        }

        [Test]
        [TestCaseSource(nameof(ValidIdentifications))]
        public void inss_is_valid(string identification)
        {
            // Arrange
            INSS inss = new (identification);

            // Act

            // Assert
            Assert.That(inss.IsValid, Is.True);
            Assert.That(inss.ElectronicVersion, Is.Not.Null);
            Assert.That(inss.PaperVersion, Is.Not.Null);
        }

        [Test]
        [TestCaseSource(nameof(Sexes))]
        public Sex? inss_sex(string identification)
        {
            // Arrange
            INSS inss = new (identification);

            // Act

            // Assert
            return inss.Sex;
        }
    }
}
