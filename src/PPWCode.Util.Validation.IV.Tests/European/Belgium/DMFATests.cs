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
    public class DMFATests : BaseTests
    {
        // TODO Add additional cases
        public static IEnumerable InvalidIdentifications
        {
            get
            {
                yield return null;
                yield return string.Empty;
                yield return "1";
                yield return "12341234";
                yield return "ABc";
            }
        }

        public static IEnumerable StrictValidIdentifications
        {
            get
            {
                yield return "DMFAP123456789A";
                yield return "DMFAT123456789A";
                yield return "DMFAA123456789A";
                yield return "DPPLP123456789A";
                yield return "DPPLT123456789A";
                yield return "DPPLA123456789A";
            }
        }

        public static IEnumerable ValidIdentifications
            => StrictValidIdentifications;

        public static IEnumerable PaperVersions
        {
            get { yield return new TestCaseData("DPPLA123456789A").Returns("DPPLA123456789A"); }
        }

        private static IEnumerable IdentificationToNumberCases
        {
            get
            {
                foreach (object invalidDMFA in InvalidIdentifications)
                {
                    yield return new TestCaseData(invalidDMFA).Returns(null);
                }

                yield return new TestCaseData("DMFAP123456789A").Returns(123456789L);
                yield return new TestCaseData("DPPLP123456789A").Returns(123456789L);

                yield return new TestCaseData("DMFAP003456789A").Returns(3456789L);
                yield return new TestCaseData("DPPLP023456789A").Returns(23456789L);
            }
        }

        [Test]
        [TestCaseSource(nameof(ValidIdentifications))]
        public void check_binairy_serializable(string identification)
        {
            // Arrange
            DMFA expected = new (identification);

            // Act
            DMFA? actual = DeepCloneUsingBinaryFormatter(expected);

            // Assert
            Assert.That(actual, Is.Not.Null);
            Assert.That(actual!.RawVersion, Is.EqualTo(expected.RawVersion));
        }

        [Test]
        [TestCaseSource(nameof(PaperVersions))]
        public string? check_paperversion(string identification)
        {
            // Arrange
            DMFA dmfa = new (identification);

            // Act

            // Assert
            Assert.That(dmfa.IsValid, Is.True);
            Assert.That(dmfa.ElectronicVersion, Is.Not.Null);
            return dmfa.PaperVersion;
        }

        [Test]
        [TestCaseSource(nameof(IdentificationToNumberCases))]
        public long? dmfa_check_as_number(string identification)
        {
            // Arrange
            DMFA dmfa = new (identification);

            // Act

            // Assert
            return dmfa.AsNumber;
        }

        [Test]
        [TestCaseSource(nameof(InvalidIdentifications))]
        public void dmfa_is_not_valid(string identification)
        {
            // Arrange
            DMFA dmfa = new (identification);

            // Act

            // Assert
            Assert.That(dmfa.IsValid, Is.False);
            Assert.That(dmfa.IsStrictValid, Is.False);
            Assert.That(dmfa.ElectronicVersion, Is.Null);
            Assert.That(dmfa.PaperVersion, Is.Null);
        }

        [Test]
        [TestCaseSource(nameof(StrictValidIdentifications))]
        public void dmfa_is_strict_valid(string identification)
        {
            // Arrange
            DMFA dmfa = new (identification);

            // Act

            // Assert
            Assert.That(dmfa.IsValid, Is.True);
            Assert.That(dmfa.IsStrictValid, Is.True);
            Assert.That(dmfa.ElectronicVersion, Is.EqualTo(dmfa.CleanedVersion));
            Assert.That(dmfa.ElectronicVersion, Is.EqualTo(dmfa.RawVersion));
            Assert.That(dmfa.PaperVersion, Is.Not.Null);
        }

        [Test]
        [TestCaseSource(nameof(ValidIdentifications))]
        public void dmfa_is_valid(string identification)
        {
            // Arrange
            DMFA dmfa = new (identification);

            // Act

            // Assert
            Assert.That(dmfa.IsValid, Is.True);
            Assert.That(dmfa.ElectronicVersion, Is.Not.Null);
            Assert.That(dmfa.PaperVersion, Is.Not.Null);
        }
    }
}
