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

using PPWCode.Util.Validation.IV.European.Netherlands;

namespace PPWCode.Util.Validation.IV.Tests.European.Netherlands
{
    [SuppressMessage("ReSharper", "InconsistentNaming", Justification = "Test")]
    [SuppressMessage("ReSharper", "UnusedMember.Local", Justification = "Test")]
    public class BSNTests : BaseTests
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
                yield return "587112189";
                yield return "566662243";
                yield return "755510859";
                yield return "545380947";
                yield return "863596502";
                yield return "894482397";
                yield return "255795373";
                yield return "794655063";
                yield return "956942933";
                yield return "210908592";
            }
        }

        public static IEnumerable ValidIdentifications
        {
            get
            {
                foreach (object bsn in StrictValidIdentifications)
                {
                    yield return bsn;
                }
            }
        }

        public static IEnumerable PaperVersions
        {
            get
            {
                foreach (object bsn in StrictValidIdentifications)
                {
                    yield return new TestCaseData(bsn).Returns(bsn);
                }
            }
        }

        [Test]
        [TestCaseSource(nameof(InvalidIdentifications))]
        public void bsn_is_not_valid(string identification)
        {
            // Arrange
            BSN bsn = new (identification);

            // Act

            // Assert
            Assert.That(bsn.IsValid, Is.False);
            Assert.That(bsn.IsStrictValid, Is.False);
            Assert.That(bsn.ElectronicVersion, Is.Null);
            Assert.That(bsn.PaperVersion, Is.Null);
        }

        [Test]
        [TestCaseSource(nameof(StrictValidIdentifications))]
        public void bsn_is_strict_valid(string identification)
        {
            // Arrange
            BSN bsn = new (identification);

            // Act

            // Assert
            Assert.That(bsn.IsValid, Is.True);
            Assert.That(bsn.IsStrictValid, Is.True);
            Assert.That(bsn.ElectronicVersion, Is.EqualTo(bsn.CleanedVersion));
            Assert.That(bsn.ElectronicVersion, Is.EqualTo(bsn.RawVersion));
            Assert.That(bsn.PaperVersion, Is.Not.Null);
        }

        [Test]
        [TestCaseSource(nameof(ValidIdentifications))]
        public void bsn_is_valid(string identification)
        {
            // Arrange
            BSN bsn = new (identification);

            // Act

            // Assert
            Assert.That(bsn.IsValid, Is.True);
            Assert.That(bsn.ElectronicVersion, Is.Not.Null);
            Assert.That(bsn.PaperVersion, Is.Not.Null);
        }

        [Test]
        [TestCaseSource(nameof(ValidIdentifications))]
        public void check_binairy_serializable(string identification)
        {
            // Arrange
            BSN expected = new (identification);

            // Act
            BSN? actual = DeepClone(expected);

            // Assert
            Assert.That(actual, Is.Not.Null);
        }

        [Test]
        [TestCaseSource(nameof(PaperVersions))]
        public string? check_paperversion(string identification)
        {
            // Arrange
            BSN bsn = new (identification);

            // Act

            // Assert
            Assert.That(bsn.IsValid, Is.True);
            Assert.That(bsn.ElectronicVersion, Is.Not.Null);
            return bsn.PaperVersion;
        }
    }
}
