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

namespace PPWCode.Util.Validation.IV.Tests
{
    [SuppressMessage("ReSharper", "InconsistentNaming", Justification = "Test")]
    [SuppressMessage("ReSharper", "UnusedMember.Local", Justification = "Test")]
    public class IBANTests : BaseTests
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

        /// <summary>
        ///     see <see href="http://www.xe.com/ibancalculator/" />
        /// </summary>
        public static IEnumerable StrictValidIndentifications
        {
            get
            {
                yield return "AL47212110090000000235698741";
                yield return "AD1200012030200359100100";
                yield return "AT611904300234573201";
                yield return "AZ21NABZ00000000137010001944";
                yield return "BH67BMAG00001299123456";
                yield return "BY86AKBB10100000002966000000";
                yield return "BE62510007547061";
                yield return "BA391290079401028494";
                yield return "BR9700360305000010009795493P1";
                yield return "BG80BNBG96611020345678";
                yield return "CR05015202001026284066";
                yield return "BG80BNBG96611020345678";
                yield return "HR1210010051863000160";
                yield return "CY17002001280000001200527600";
                yield return "CZ6508000000192000145399";
                yield return "DK5000400440116243";
                yield return "DO28BAGR00000001212453611324";
                yield return "TL380080012345678910157";
                yield return "EE382200221020145685";
                yield return "FO9754320388899944";
                yield return "FI2112345600000785";
                yield return "FR1420041010050500013M02606";
                yield return "GE29NB0000000101904917";
                yield return "DE89370400440532013000";
                yield return "GI75NWBK000000007099453";
                yield return "GR1601101250000000012300695";
                yield return "GL5604449876543210";
                yield return "GT82TRAJ01020000001210029690";
                yield return "HU42117730161111101800000000";
                yield return "IS140159260076545510730339";
                yield return "IE29AIBK93115212345678";
                yield return "IL620108000000099999999";
                yield return "IT60X0542811101000000123456";
                yield return "JO94CBJO0010000000000131000302";
                yield return "KZ86125KZT5004100100";
                yield return "XK051212012345678906";
                yield return "KW81CBKU0000000000001234560101";
                yield return "LV80BANK0000435195001";
                yield return "LB62099900000001001901229114";
                yield return "LI21088100002324013AA";
                yield return "LT121000011101001000";
                yield return "LU280019400644750000";
                yield return "MK07250120000058984";
                yield return "MT84MALT011000012345MTLCAST001S";
                yield return "MR1300020001010000123456753";
                yield return "MU17BOMM0101101030300200000MUR";
                yield return "MC9320052222100112233M44555";
                yield return "MD24AG000225100013104168";
                yield return "ME25505000012345678951";
                yield return "NL39RABO0300065264";
                yield return "NO9386011117947";
                yield return "PK36SCBL0000001123456702";
                yield return "PS92PALS000000000400123456702";
                yield return "PL60102010260000042270201111";
                yield return "PT50000201231234567890154";
                yield return "QA58DOHB00001234567890ABCDEFG";
                yield return "RO49AAAA1B31007593840000";
                yield return "SM86U0322509800000000270100";
                yield return "SA0380000000608010167519";
                yield return "RS35260005601001611379";
                yield return "SK3112000000198742637541";
                yield return "SI56191000000123438";
                yield return "ES8023100001180000012345";
                yield return "SE3550000000054910000003";
                yield return "CH9300762011623852957";
                yield return "TN5910006035183598478831";
                yield return "TR330006100519786457841326";
                yield return "AE070331234567890123456";
                yield return "GB29NWBK60161331926819";
                yield return "VG96VPVG0000012345678901";
            }
        }

        public static IEnumerable ValidIdentifications
        {
            get
            {
                foreach (object iban in StrictValidIndentifications)
                {
                    yield return iban;
                }

                yield return "AL47 2121 1009 0000 0002 3569 8741";
                yield return "AD12 0001 2030 2003 5910 0100";
                yield return "AT61 1904 3002 3457 3201";
                yield return "AZ21 NABZ 0000 0000 1370 1000 1944";
                yield return "BH67 BMAG 0000 1299 1234 56";
                yield return "BY86 AKBB 1010 0000 0029 6600 0000";
                yield return "BE62 5100 0754 7061";
                yield return "BA39 1290 0794 0102 8494";
                yield return "BR97 0036 0305 0000 1000 9795 493P 1";
                yield return "BG80 BNBG 9661 1020 3456 78";
                yield return "CR05 0152 0200 1026 2840 66";
                yield return "BG80 BNBG 9661 1020 3456 78";
                yield return "HR12 1001 0051 8630 0016 0";
                yield return "CY17 0020 0128 0000 0012 0052 7600";
                yield return "CZ65 0800 0000 1920 0014 5399";
                yield return "DK50 0040 0440 1162 43";
                yield return "DO28 BAGR 0000 0001 2124 5361 1324";
                yield return "TL38 0080 0123 4567 8910 157";
                yield return "EE38 2200 2210 2014 5685";
                yield return "FO97 5432 0388 8999 44";
                yield return "FI21 1234 5600 0007 85";
                yield return "FR14 2004 1010 0505 0001 3M02 606";
                yield return "GE29 NB00 0000 0101 9049 17";
                yield return "DE89 3704 0044 0532 0130 00";
                yield return "GI75 NWBK 0000 0000 7099 453";
                yield return "GR16 0110 1250 0000 0001 2300 695";
                yield return "GL56 0444 9876 5432 10";
                yield return "GT82 TRAJ 0102 0000 0012 1002 9690";
                yield return "HU42 1177 3016 1111 1018 0000 0000";
                yield return "IS14 0159 2600 7654 5510 7303 39";
                yield return "IE29 AIBK 9311 5212 3456 78";
                yield return "IL62 0108 0000 0009 9999 999";
                yield return "IT60 X054 2811 1010 0000 0123 456";
                yield return "JO94 CBJO 0010 0000 0000 0131 0003 02";
                yield return "KZ86 125K ZT50 0410 0100";
                yield return "XK05 1212 0123 4567 8906";
                yield return "KW81 CBKU 0000 0000 0000 1234 5601 01";
                yield return "LV80 BANK 0000 4351 9500 1";
                yield return "LB62 0999 0000 0001 0019 0122 9114";
                yield return "LI21 0881 0000 2324 013A A";
                yield return "LT12 1000 0111 0100 1000";
                yield return "LU28 0019 4006 4475 0000";
                yield return "MK072 5012 0000 0589 84";
                yield return "MT84 MALT 0110 0001 2345 MTLC AST0 01S";
                yield return "MR13 0002 0001 0100 0012 3456 753";
                yield return "MU17 BOMM 0101 1010 3030 0200 000M UR";
                yield return "MC93 2005 2222 1001 1223 3M44 555";
                yield return "MD24 AG00 0225 1000 1310 4168";
                yield return "ME25 5050 0001 2345 6789 51";
                yield return "NL39 RABO 0300 0652 64";
                yield return "NO93 8601 1117 947";
                yield return "PK36 SCBL 0000 0011 2345 6702";
                yield return "PS92 PALS 0000 0000 0400 1234 5670 2";
                yield return "PL60 1020 1026 0000 0422 7020 1111";
                yield return "PT50 0002 0123 1234 5678 9015 4";
                yield return "QA58 DOHB 0000 1234 5678 90AB CDEF G";
                yield return "RO49 AAAA 1B31 0075 9384 0000";
                yield return "SM86 U032 2509 8000 0000 0270 100";
                yield return "SA03 8000 0000 6080 1016 7519";
                yield return "RS35 2600 0560 1001 6113 79";
                yield return "SK31 1200 0000 1987 4263 7541";
                yield return "SI56 1910 0000 0123 438";
                yield return "ES80 2310 0001 1800 0001 2345";
                yield return "SE35 5000 0000 0549 1000 0003";
                yield return "CH93 0076 2011 6238 5295 7";
                yield return "TN59 1000 6035 1835 9847 8831";
                yield return "TR33 0006 1005 1978 6457 8413 26";
                yield return "AE07 0331 2345 6789 0123 456";
                yield return "GB29 NWBK 6016 1331 9268 19";
                yield return "VG96 VPVG 0000 0123 4567 8901";
            }
        }

        private static IEnumerable ValidBEIndentifications
            => ValidIdentifications
                .Cast<string>()
                .Where(i => string.Equals(i.Substring(0, 2), "BE", StringComparison.InvariantCulture));

        public static IEnumerable PaperVersions
        {
            get
            {
                yield return new TestCaseData(" NO938601 1117 947").Returns("NO93 8601 1117 947");
                yield return new TestCaseData("QA58DOHB 0000 12345678 90AB CDEFG").Returns("QA58 DOHB 0000 1234 5678 90AB CDEF G");
            }
        }

        [Test]
        [TestCaseSource(nameof(ValidIdentifications))]
        public void check_binairy_serializable(string identification)
        {
            // Arrange
            IBAN expected = new (identification);

            // Act
            IBAN? actual = DeepCloneUsingBinaryFormatter(expected);

            // Assert
            Assert.That(actual, Is.Not.Null);
            Assert.That(actual!.RawVersion, Is.EqualTo(expected.RawVersion));
        }

        [Test]
        [TestCaseSource(nameof(PaperVersions))]
        public string? check_paperversion(string identification)
        {
            // Arrange
            IBAN bban = new (identification);

            // Act

            // Assert
            Assert.That(bban.IsValid, Is.True);
            Assert.That(bban.ElectronicVersion, Is.Not.Null);
            return bban.PaperVersion;
        }

        [Test]
        [TestCaseSource(nameof(ValidBEIndentifications))]
        public void iban_can_convert_valid_BE_identifications_to_bban(string identification)
        {
            // Arrange
            IBAN iban = new (identification);
            Assert.That(iban.IsValid, Is.True);

            // Act
            BBAN? actual = iban.AsBBAN;

            // Assert
            Assert.That(actual, Is.Not.Null);
            Assert.That(actual!.IsValid, Is.True);
            Assert.That(actual.IsStrictValid, Is.True);
        }

        [Test]
        [TestCaseSource(nameof(InvalidIdentifications))]
        public void iban_can_not_convert_invalid_identifications_to_bban(string identification)
        {
            // Arrange
            IBAN iban = new (identification);
            Assert.That(iban.IsValid, Is.False);

            // Act
            BBAN? actual = iban.AsBBAN;

            // Assert
            Assert.That(actual, Is.Null);
        }

        [Test]
        [TestCaseSource(nameof(InvalidIdentifications))]
        public void iban_is_not_valid(string identification)
        {
            // Arrange
            IBAN iban = new (identification);

            // Act

            // Assert
            Assert.That(iban.IsValid, Is.False);
            Assert.That(iban.IsStrictValid, Is.False);
            Assert.That(iban.ElectronicVersion, Is.Null);
            Assert.That(iban.PaperVersion, Is.Null);
        }

        [Test]
        [TestCaseSource(nameof(StrictValidIndentifications))]
        public void iban_is_strict_valid(string identification)
        {
            // Arrange
            IBAN iban = new (identification);

            // Act

            // Assert
            Assert.That(iban.IsValid, Is.True);
            Assert.That(iban.IsStrictValid, Is.True);
            Assert.That(iban.ElectronicVersion, Is.EqualTo(iban.CleanedVersion));
            Assert.That(iban.ElectronicVersion, Is.EqualTo(iban.RawVersion));
            Assert.That(iban.PaperVersion, Is.Not.Null);
        }

        [Test]
        [TestCaseSource(nameof(ValidIdentifications))]
        public void iban_is_valid(string identification)
        {
            // Arrange
            IBAN iban = new (identification);

            // Act

            // Assert
            Assert.That(iban.IsValid, Is.True);
            Assert.That(iban.ElectronicVersion, Is.Not.Null);
            Assert.That(iban.PaperVersion, Is.Not.Null);
        }
    }
}
