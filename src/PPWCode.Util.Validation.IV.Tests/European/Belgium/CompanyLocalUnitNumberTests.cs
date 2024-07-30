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
    public class CompanyLocalUnitNumberTests : BaseTests
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
            get { yield return "2069315153"; }
        }

        public static IEnumerable ValidIdentifications
        {
            get
            {
                foreach (object companyLocalUnitNumber in StrictValidIdentifications)
                {
                    yield return companyLocalUnitNumber;
                }

                foreach (CompanyLocalUnitNumber companyLocalUnitNumber in CompanyLocalUnitNumber.ValidFictiveNumbers)
                {
                    yield return companyLocalUnitNumber.RawVersion;
                }

                yield return "2.069.315.153";
            }
        }

        public static IEnumerable PaperVersions
        {
            get { yield return new TestCaseData("2069315153").Returns("2.069.315.153"); }
        }

        [Test]
        [TestCaseSource(nameof(ValidIdentifications))]
        public void check_binairy_serializable(string identification)
        {
            // Arrange
            CompanyLocalUnitNumber expected = new (identification);

            // Act
            CompanyLocalUnitNumber? actual = DeepCloneUsingBinaryFormatter(expected);

            // Assert
            Assert.That(actual, Is.Not.Null);
            Assert.That(actual!.RawVersion, Is.EqualTo(expected.RawVersion));
        }

        [Test]
        [TestCaseSource(nameof(PaperVersions))]
        public string? check_paperversion(string identification)
        {
            // Arrange
            CompanyLocalUnitNumber companyLocalUnitNumber = new (identification);

            // Act

            // Assert
            Assert.That(companyLocalUnitNumber.IsValid, Is.True);
            Assert.That(companyLocalUnitNumber.ElectronicVersion, Is.Not.Null);
            return companyLocalUnitNumber.PaperVersion;
        }

        [Test]
        [TestCaseSource(nameof(InvalidIdentifications))]
        public void companyLocalUnitNumber_is_not_valid(string identification)
        {
            // Arrange
            CompanyLocalUnitNumber companyLocalUnitNumber = new (identification);

            // Act

            // Assert
            Assert.That(companyLocalUnitNumber.IsValid, Is.False);
            Assert.That(companyLocalUnitNumber.IsStrictValid, Is.False);
            Assert.That(companyLocalUnitNumber.ElectronicVersion, Is.Null);
            Assert.That(companyLocalUnitNumber.PaperVersion, Is.Null);
        }

        [Test]
        [TestCaseSource(nameof(StrictValidIdentifications))]
        public void companyLocalUnitNumber_is_strict_valid(string identification)
        {
            // Arrange
            CompanyLocalUnitNumber companyLocalUnitNumber = new (identification);

            // Act

            // Assert
            Assert.That(companyLocalUnitNumber.IsValid, Is.True);
            Assert.That(companyLocalUnitNumber.IsStrictValid, Is.True);
            Assert.That(companyLocalUnitNumber.ElectronicVersion, Is.EqualTo(companyLocalUnitNumber.CleanedVersion));
            Assert.That(companyLocalUnitNumber.ElectronicVersion, Is.EqualTo(companyLocalUnitNumber.RawVersion));
            Assert.That(companyLocalUnitNumber.PaperVersion, Is.Not.Null);
        }

        [Test]
        [TestCaseSource(nameof(ValidIdentifications))]
        public void companyLocalUnitNumber_is_valid(string identification)
        {
            // Arrange
            CompanyLocalUnitNumber companyLocalUnitNumber = new (identification);

            // Act

            // Assert
            Assert.That(companyLocalUnitNumber.IsValid, Is.True);
            Assert.That(companyLocalUnitNumber.ElectronicVersion, Is.Not.Null);
            Assert.That(companyLocalUnitNumber.PaperVersion, Is.Not.Null);
        }
    }
}
