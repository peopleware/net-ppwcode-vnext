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
    public class OGMTests : BaseTests
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
            get { yield return "022018202879"; }
        }

        public static IEnumerable ValidIdentifications
        {
            get
            {
                foreach (object ogm in StrictValidIdentifications)
                {
                    yield return ogm;
                }

                yield return "022018202879 ";
                yield return "022/0182/02879";
                yield return "+++022018202879";
                yield return "022018202879+++";
                yield return "+++022018202879+++";
                yield return "+++022/0182/02879+++";
            }
        }

        public static IEnumerable PaperVersions
        {
            get
            {
                yield return new TestCaseData("022018202879").Returns("+++022/0182/02879+++");
                yield return new TestCaseData("022/0182/02879").Returns("+++022/0182/02879+++");
            }
        }

        [Test]
        [TestCaseSource(nameof(ValidIdentifications))]
        public void check_binairy_serializable(string identification)
        {
            // Arrange
            OGM ogm = new (identification);

            // Act
            OGM? actual = DeepCloneUsingBinaryFormatter(ogm);

            // Assert
            Assert.That(actual, Is.Not.Null);
            Assert.That(actual!.RawVersion, Is.EqualTo(ogm.RawVersion));
        }

        [Test]
        [TestCaseSource(nameof(PaperVersions))]
        public string? check_paperversion(string identification)
        {
            // Arrange
            OGM ogm = new (identification);

            // Act

            // Assert
            Assert.That(ogm.IsValid, Is.True);
            Assert.That(ogm.ElectronicVersion, Is.Not.Null);
            return ogm.PaperVersion;
        }

        [Test]
        [TestCaseSource(nameof(InvalidIdentifications))]
        public void ogm_is_not_valid(string identification)
        {
            // Arrange
            OGM ogm = new (identification);

            // Act

            // Assert
            Assert.That(ogm.IsValid, Is.False);
            Assert.That(ogm.IsStrictValid, Is.False);
            Assert.That(ogm.ElectronicVersion, Is.Null);
            Assert.That(ogm.PaperVersion, Is.Null);
        }

        [Test]
        [TestCaseSource(nameof(StrictValidIdentifications))]
        public void ogm_is_strict_valid(string identification)
        {
            // Arrange
            OGM ogm = new (identification);

            // Act

            // Assert
            Assert.That(ogm.IsValid, Is.True);
            Assert.That(ogm.IsStrictValid, Is.True);
            Assert.That(ogm.ElectronicVersion, Is.EqualTo(ogm.CleanedVersion));
            Assert.That(ogm.ElectronicVersion, Is.EqualTo(ogm.RawVersion));
            Assert.That(ogm.PaperVersion, Is.Not.Null);
        }

        [Test]
        [TestCaseSource(nameof(ValidIdentifications))]
        public void ogm_is_valid(string identification)
        {
            // Arrange
            OGM ogm = new (identification);

            // Act

            // Assert
            Assert.That(ogm.IsValid, Is.True);
            Assert.That(ogm.ElectronicVersion, Is.Not.Null);
            Assert.That(ogm.PaperVersion, Is.Not.Null);
        }
    }
}
