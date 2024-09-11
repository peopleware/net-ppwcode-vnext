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

namespace PPWCode.Util.Validation.IV.Tests
{
    [SuppressMessage("ReSharper", "InconsistentNaming", Justification = "Test")]
    [SuppressMessage("ReSharper", "UnusedMember.Local", Justification = "Test")]
    public class BICTests : BaseTests
    {
        private static IEnumerable InvalidIdentifications
        {
            get
            {
                yield return null;
                yield return string.Empty;
                yield return "1";
                yield return "12341234";
                yield return "KREDXXBB";
                yield return "KREDBEBB1";
                yield return "KREDBEBB12";
            }
        }

        private static IEnumerable StrictValidIdentifications
        {
            get
            {
                yield return "DEUTDEFF";
                yield return "DEUTDEFF500";
                yield return "NEDSZAJJ";
                yield return "NEDSZAJJXXX";
                yield return "DABADKKK";
                yield return "UNCRITMM";
                yield return "DSBACNBXSHA";
                yield return "BNORPHMM";
                yield return "BEASUS33XXX";
                yield return "BEASCNXXXXX";
                yield return "BOFAUS3N";
                yield return "BSAMLKLXXXX";
                yield return "KREDBEBB";
            }
        }

        private static IEnumerable ValidIdentifications
        {
            get
            {
                foreach (object bic in StrictValidIdentifications)
                {
                    yield return bic;
                }

                yield return "DEU TDeFF";
                yield return " DEUTDEF f500";
                yield return "NEDSZ AJJ ";
            }
        }

        private static IEnumerable PaperVersions
        {
            get
            {
                yield return new TestCaseData(" BNO RP HMM ").Returns("BNORPHMM");
                yield return new TestCaseData("BeASuS33xxx").Returns("BEASUS33XXX");
            }
        }

        [Test]
        [TestCaseSource(nameof(InvalidIdentifications))]
        public void bic_is_not_valid(string identification)
        {
            // Arrange
            BIC bic = new (identification);

            // Act

            // Assert
            Assert.That(bic.IsValid, Is.False);
            Assert.That(bic.IsStrictValid, Is.False);
            Assert.That(bic.ElectronicVersion, Is.Null);
            Assert.That(bic.PaperVersion, Is.Null);
        }

        [Test]
        [TestCaseSource(nameof(StrictValidIdentifications))]
        public void bic_is_strict_valid(string identification)
        {
            // Arrange
            BIC bic = new (identification);

            // Act

            // Assert
            Assert.That(bic.IsValid, Is.True);
            Assert.That(bic.IsStrictValid, Is.True);
            Assert.That(bic.ElectronicVersion, Is.EqualTo(bic.CleanedVersion));
            Assert.That(bic.ElectronicVersion, Is.EqualTo(bic.RawVersion));
            Assert.That(bic.PaperVersion, Is.Not.Null);
        }

        [Test]
        [TestCaseSource(nameof(ValidIdentifications))]
        public void bic_is_valid(string identification)
        {
            // Arrange
            BIC bic = new (identification);

            // Act

            // Assert
            Assert.That(bic.IsValid, Is.True);
            Assert.That(bic.ElectronicVersion, Is.Not.Null);
            Assert.That(bic.PaperVersion, Is.Not.Null);
        }

        [Test]
        [TestCaseSource(nameof(ValidIdentifications))]
        public void check_binairy_serializable(string identification)
        {
            // Arrange
            BIC expected = new (identification);

            // Act
            BIC? actual = DeepClone(expected);

            // Assert
            Assert.That(actual, Is.Not.Null);
            Assert.That(actual!.RawVersion, Is.EqualTo(expected.RawVersion));
        }

        [Test]
        [TestCaseSource(nameof(PaperVersions))]
        public string? check_paperversion(string identification)
        {
            // Arrange
            BIC bic = new (identification);

            // Act

            // Assert
            Assert.That(bic.IsValid, Is.True);
            Assert.That(bic.ElectronicVersion, Is.Not.Null);
            return bic.PaperVersion;
        }
    }
}
