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
    public class TemporaryRSZTests : BaseTests
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
            get { yield return "5105009119"; }
        }

        public static IEnumerable ValidIdentifications
        {
            get
            {
                foreach (object kbo in StrictValidIdentifications)
                {
                    yield return kbo;
                }

                yield return "Temp. RSZ 5105009119";
                yield return "51050091.19";
            }
        }

        public static IEnumerable PaperVersions
        {
            get
            {
                yield return new TestCaseData("5105009119").Returns("51050091-19");
                yield return new TestCaseData("BE 5105009119").Returns("51050091-19");
            }
        }

        [Test]
        [TestCaseSource(nameof(ValidIdentifications))]
        public void check_binairy_serializable(string identification)
        {
            // Arrange
            INSS expected = new (identification);

            // Act
            INSS? actual = DeepClone(expected);

            // Assert
            Assert.That(actual, Is.Not.Null);
            Assert.That(actual!.RawVersion, Is.EqualTo(expected.RawVersion));
        }

        [Test]
        [TestCaseSource(nameof(PaperVersions))]
        public string? check_paperversion(string identification)
        {
            // Arrange
            TemporaryRSZ temporaryRSZ = new (identification);

            // Act

            // Assert
            Assert.That(temporaryRSZ.IsValid, Is.True);
            Assert.That(temporaryRSZ.ElectronicVersion, Is.Not.Null);
            return temporaryRSZ.PaperVersion;
        }

        [Test]
        [TestCaseSource(nameof(InvalidIdentifications))]
        public void temporary_rsz_is_not_valid(string identification)
        {
            // Arrange
            TemporaryRSZ temporaryRsz = new (identification);

            // Act

            // Assert
            Assert.That(temporaryRsz.IsValid, Is.False);
            Assert.That(temporaryRsz.IsStrictValid, Is.False);
            Assert.That(temporaryRsz.ElectronicVersion, Is.Null);
            Assert.That(temporaryRsz.PaperVersion, Is.Null);
        }

        [Test]
        [TestCaseSource(nameof(StrictValidIdentifications))]
        public void temporary_rsz_is_strict_valid(string identification)
        {
            // Arrange
            TemporaryRSZ temporaryRsz = new (identification);

            // Act

            // Assert
            Assert.That(temporaryRsz.IsValid, Is.True);
            Assert.That(temporaryRsz.IsStrictValid, Is.True);
            Assert.That(temporaryRsz.ElectronicVersion, Is.EqualTo(temporaryRsz.CleanedVersion));
            Assert.That(temporaryRsz.ElectronicVersion, Is.EqualTo(temporaryRsz.RawVersion));
            Assert.That(temporaryRsz.PaperVersion, Is.Not.Null);
        }

        [Test]
        [TestCaseSource(nameof(ValidIdentifications))]
        public void temporary_rsz_is_valid(string identification)
        {
            // Arrange
            TemporaryRSZ temporaryRsz = new (identification);

            // Act

            // Assert
            Assert.That(temporaryRsz.IsValid, Is.True);
            Assert.That(temporaryRsz.ElectronicVersion, Is.Not.Null);
            Assert.That(temporaryRsz.PaperVersion, Is.Not.Null);
        }
    }
}
