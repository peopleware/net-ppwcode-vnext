// Copyright 2025 by PeopleWare n.v..
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

using PPWCode.Util.Collection.I;
using PPWCode.Vernacular.Contracts.I;

namespace PPWCode.Util.Time.I.Tests;

[TestFixture]
[SuppressMessage("ReSharper", "InconsistentNaming", Justification = "Tests")]
public abstract class PeriodHistoryTests<TPeriod, T> : BasePeriodTests<TPeriod, T>
    where TPeriod : class, IPeriod<T>
    where T : struct, IComparable<T>, IEquatable<T>
{
    public static IEnumerable CreationCases
    {
        get
        {
            yield return
                new TestCaseData(new StringArray(["X__", "_X_"]))
                    .Returns("XX");
        }
    }

    public static IEnumerable IntersectWithCases
    {
        get
        {
            yield return
                new TestCaseData(
                    string.Empty,
                    string.Empty,
                    string.Empty);
            yield return
                new TestCaseData(
                    string.Empty,
                    "X",
                    string.Empty);
            yield return
                new TestCaseData(
                    "X",
                    "X",
                    "X");
            yield return
                new TestCaseData(
                    "X",
                    "_",
                    "_");
            yield return
                new TestCaseData(
                    "_",
                    "X",
                    "_");
            yield return
                new TestCaseData(
                    "XX",
                    "XX",
                    "XX");
            yield return
                new TestCaseData(
                    "__",
                    "XX",
                    "__");
            yield return
                new TestCaseData(
                    "XX",
                    "__",
                    "__");
            yield return
                new TestCaseData(
                    "X_",
                    "_X",
                    "__");
            yield return
                new TestCaseData(
                    "_X",
                    "X_",
                    "__");
            yield return
                new TestCaseData(
                    "_XX_",
                    "X__X",
                    "____");
            yield return
                new TestCaseData(
                    "_XX_",
                    "X_XX",
                    "__X_");
            yield return
                new TestCaseData(
                    "__XXXXXXXXXX__XXXX_XXXX",
                    "XXXX_XXX__XXXXXX_______",
                    "__XX_XXX__XX__XX_______");
            yield return
                new TestCaseData(
                    "XXXX__________XXX",
                    "______XXX__XX____",
                    "_________________");
            yield return
                new TestCaseData(
                    "XXXXXXXXX__XXX__XXX",
                    "XXXXXXXXX__XXX__XXX",
                    "XXXXXXXXX__XXX__XXX");
            yield return
                new TestCaseData(
                    "XXXXXXXXXXXXXXXXXXX",
                    "_XXXX__XX___XXXXX__",
                    "_XXXX__XX___XXXXX__");
            yield return
                new TestCaseData(
                    "____XXXXX___",
                    "XX________XX",
                    "____________");
        }
    }

    public static IEnumerable ExceptWithCases
    {
        get
        {
            yield return
                new TestCaseData(
                    "__XXX",
                    "X____",
                    "__XXX");

            yield return
                new TestCaseData(
                    "__XXX",
                    "XX___",
                    "__XXX");

            yield return
                new TestCaseData(
                    "__XXX",
                    "XXX__",
                    "___XX");

            yield return
                new TestCaseData(
                    "XXXX_",
                    "XX___",
                    "__XX_");

            yield return
                new TestCaseData(
                    "XXXX_",
                    "_X___",
                    "X_XX_");

            yield return
                new TestCaseData(
                    "XXXX_",
                    "_XXX_",
                    "X____");

            yield return
                new TestCaseData(
                    "XXX__",
                    "__XX_",
                    "XX___");

            yield return
                new TestCaseData(
                    "XXX__",
                    "___XX",
                    "XXX__");

            yield return
                new TestCaseData(
                    "_XXXXXX_",
                    "__X_X___",
                    "_X_X_XX_");

            yield return
                new TestCaseData(
                    "_XXX_XXX_",
                    "__X_XX___",
                    "_X_X__XX_");

            yield return
                new TestCaseData(
                    "_XXX_XXX_",
                    "__X_X____",
                    "_X_X_XXX_");

            yield return
                new TestCaseData(
                    "_XXX_XXX_XXX",
                    "__X_X___XX__",
                    "_X_X_XXX__XX");

            yield return
                new TestCaseData(
                    "_XXX_XXX_XXX_XXX_",
                    "__X_X___XX_______",
                    "_X_X_XXX__XX_XXX_");

            yield return
                new TestCaseData(
                    "__XX_XX_XXXX",
                    "XXXXXXXXXX__",
                    "__________XX");
        }
    }

    protected abstract PeriodHistory<TPeriod, T> CreatePeriodHistory(IEnumerable<TPeriod> periods);

    [TestCase("XXXXX.", "X.")]
    [TestCase("X__X_XXX.", "X__X_.")]
    [TestCase("_XXX.", "_.")]
    [TestCase("XXX", "XXX")]
    [TestCase("XXX.", "X.")]
    [TestCase(".X", ".X")]
    [TestCase(".XXX.", ".")]
    [TestCase("._", "._")]
    [TestCase(".XXX_", ".XXX")]
    [TestCase("_.", "_.")]
    [TestCase(".", ".")]
    [TestCase("XX___", "XX")]
    [TestCase("XX___X_", "XX___X")]
    [TestCase("_", "")]
    [TestCase("____", "")]
    [TestCase(".________", "._")]
    [TestCase(".XXXX___", ".XXXX")]
    [TestCase("._.", "._.")]
    [TestCase(".__.", ".__.")]
    [TestCase("._X_.", "._X_.")]
    public void test_canonicalize_periods_string(string original, string expected)
    {
        Assert.That(CanonicalizePeriodsString(original), Is.EqualTo(expected));
    }

    [Test]
    public void test_get_period_at_with_no_periods()
    {
        // Arrange
        PeriodHistory<TPeriod, T> periodHistory = CreatePeriodHistory([]);

        // Act
        TPeriod? period = periodHistory.GetPeriodAt(CreatePoint(2018, 1, 15));

        // Assert
        Assert.That(period, Is.Null);
    }

    [Test]
    public void test_get_period_at_with_single_period()
    {
        // Arrange
        TPeriod period = CreatePeriod(CreatePoint(2018, 1, 1), CreatePoint(2018, 2, 1));
        PeriodHistory<TPeriod, T> periodHistory = CreatePeriodHistory([period]);

        // Act
        TPeriod? periodBefore = periodHistory.GetPeriodAt(CreatePoint(2017, 12, 31));
        TPeriod? periodOnStart = periodHistory.GetPeriodAt(CreatePoint(2018, 1, 1));
        TPeriod? periodMiddle = periodHistory.GetPeriodAt(CreatePoint(2018, 1, 15));
        TPeriod? periodOnEnd = periodHistory.GetPeriodAt(CreatePoint(2018, 2, 1));
        TPeriod? periodAfter = periodHistory.GetPeriodAt(CreatePoint(2018, 2, 5));

        // Assert
        Assert.That(periodBefore, Is.Null);
        Assert.That(periodOnStart, Is.EqualTo(period));
        Assert.That(periodMiddle, Is.EqualTo(period));
        Assert.That(periodOnEnd, Is.Null);
        Assert.That(periodAfter, Is.Null);
    }

    [Test]
    public void test_get_period_at_with_multiple_periods()
    {
        // Arrange
        TPeriod period1 = CreatePeriod(CreatePoint(2018, 1, 1), CreatePoint(2018, 2, 1));
        TPeriod period2 = CreatePeriod(CreatePoint(2018, 3, 1), CreatePoint(2018, 4, 1));
        PeriodHistory<TPeriod, T> periodHistory = CreatePeriodHistory([period1, period2]);

        // Act
        TPeriod? periodBefore = periodHistory.GetPeriodAt(CreatePoint(2017, 12, 31));
        TPeriod? periodOnFirst = periodHistory.GetPeriodAt(CreatePoint(2018, 1, 10));
        TPeriod? periodBetween = periodHistory.GetPeriodAt(CreatePoint(2018, 2, 15));
        TPeriod? periodOnSecond = periodHistory.GetPeriodAt(CreatePoint(2018, 3, 10));
        TPeriod? periodAfter = periodHistory.GetPeriodAt(CreatePoint(2018, 4, 15));

        // Assert
        Assert.That(periodBefore, Is.Null);
        Assert.That(periodOnFirst, Is.EqualTo(period1));
        Assert.That(periodBetween, Is.Null);
        Assert.That(periodOnSecond, Is.EqualTo(period2));
        Assert.That(periodAfter, Is.Null);
    }

    [Test]
    public void test_get_periods_overlapping_at_with_no_periods()
    {
        // Arrange
        PeriodHistory<TPeriod, T> history = CreatePeriodHistory([]);

        // Act
        IList<TPeriod> periods = history.GetPeriodsOverlappingAt(CreatePoint(2018, 1, 15), CreatePoint(2018, 5, 15));

        // Assert
        Assert.That(periods, Is.Empty);
    }

    [Test]
    public void test_get_periods_overlapping_at_with_single_period()
    {
        // Arrange
        TPeriod period = CreatePeriod(CreatePoint(2018, 1, 1), CreatePoint(2018, 2, 1));
        PeriodHistory<TPeriod, T> periodHistory = CreatePeriodHistory([period]);

        // Act
        IList<TPeriod> periodsBefore = periodHistory.GetPeriodsOverlappingAt(CreatePoint(2017, 5, 1), CreatePoint(2018, 1, 1));
        IList<TPeriod> periodsContaining = periodHistory.GetPeriodsOverlappingAt(CreatePoint(2018, 1, 10), CreatePoint(2018, 1, 15));
        IList<TPeriod> periodsCovering = periodHistory.GetPeriodsOverlappingAt(CreatePoint(2017, 12, 10), CreatePoint(2018, 3, 15));
        IList<TPeriod> periodsOverlappingStart = periodHistory.GetPeriodsOverlappingAt(CreatePoint(2017, 12, 10), CreatePoint(2018, 1, 15));
        IList<TPeriod> periodsOverlappingEnd = periodHistory.GetPeriodsOverlappingAt(CreatePoint(2018, 1, 25), CreatePoint(2018, 3, 15));
        IList<TPeriod> periodsAfter = periodHistory.GetPeriodsOverlappingAt(CreatePoint(2018, 2, 1), CreatePoint(2018, 2, 5));

        // Assert
        Assert.That(periodsBefore, Is.Empty);
        Assert.That(periodsContaining, Has.Count.EqualTo(1));
        Assert.That(periodsContaining.Single(), Is.EqualTo(period));
        Assert.That(periodsCovering, Has.Count.EqualTo(1));
        Assert.That(periodsCovering.Single(), Is.EqualTo(period));
        Assert.That(periodsOverlappingStart, Has.Count.EqualTo(1));
        Assert.That(periodsOverlappingStart.Single(), Is.EqualTo(period));
        Assert.That(periodsOverlappingEnd, Has.Count.EqualTo(1));
        Assert.That(periodsOverlappingEnd.Single(), Is.EqualTo(period));
        Assert.That(periodsAfter, Is.Empty);
    }

    [Test]
    public void test_get_periods_overlapping_at_with_multiple_periods()
    {
        // Arrange
        TPeriod period1 = CreatePeriod(CreatePoint(2018, 1, 1), CreatePoint(2018, 2, 1));
        TPeriod period2 = CreatePeriod(CreatePoint(2018, 3, 1), CreatePoint(2018, 4, 1));
        PeriodHistory<TPeriod, T> history = CreatePeriodHistory([period1, period2]);

        // Act
        IList<TPeriod> periodsBefore = history.GetPeriodsOverlappingAt(CreatePoint(2017, 5, 1), CreatePoint(2018, 1, 1));
        IList<TPeriod> periodsContaining1 = history.GetPeriodsOverlappingAt(CreatePoint(2018, 1, 10), CreatePoint(2018, 1, 15));
        IList<TPeriod> periodsContaining2 = history.GetPeriodsOverlappingAt(CreatePoint(2018, 3, 10), CreatePoint(2018, 3, 15));
        IList<TPeriod> periodsCovering1 = history.GetPeriodsOverlappingAt(CreatePoint(2017, 12, 10), CreatePoint(2018, 2, 15));
        IList<TPeriod> periodsCovering2 = history.GetPeriodsOverlappingAt(CreatePoint(2018, 2, 10), CreatePoint(2018, 4, 21));
        IList<TPeriod> periodsCoveringAll = history.GetPeriodsOverlappingAt(CreatePoint(2017, 12, 10), CreatePoint(2018, 5, 15));
        IList<TPeriod> periodsOverlapping12 = history.GetPeriodsOverlappingAt(CreatePoint(2018, 1, 23), CreatePoint(2018, 3, 5));
        IList<TPeriod> periodsBetween12 = history.GetPeriodsOverlappingAt(CreatePoint(2018, 2, 23), CreatePoint(2018, 2, 25));
        IList<TPeriod> periodsAfter = history.GetPeriodsOverlappingAt(CreatePoint(2018, 4, 1), CreatePoint(2018, 2, 5));

        // Assert
        Assert.That(periodsBefore, Is.Empty);
        Assert.That(periodsContaining1, Has.Count.EqualTo(1));
        Assert.That(periodsContaining1.Single(), Is.EqualTo(period1));
        Assert.That(periodsContaining2, Has.Count.EqualTo(1));
        Assert.That(periodsContaining2.Single(), Is.EqualTo(period2));
        Assert.That(periodsCovering1, Has.Count.EqualTo(1));
        Assert.That(periodsCovering1.Single(), Is.EqualTo(period1));
        Assert.That(periodsCovering2, Has.Count.EqualTo(1));
        Assert.That(periodsCovering2.Single(), Is.EqualTo(period2));
        Assert.That(periodsCoveringAll, Has.Count.EqualTo(2));
        Assert.That(periodsCoveringAll, Contains.Item(period1));
        Assert.That(periodsCoveringAll, Contains.Item(period2));
        Assert.That(periodsOverlapping12, Has.Count.EqualTo(2));
        Assert.That(periodsOverlapping12, Contains.Item(period1));
        Assert.That(periodsOverlapping12, Contains.Item(period2));
        Assert.That(periodsBetween12, Is.Empty);
        Assert.That(periodsAfter, Is.Empty);
    }

    [Test]
    public void test_get_periods_overlapping_with_open_ended_period()
    {
        // Arrange
        TPeriod period = CreatePeriod(CreatePoint(2018, 1, 1), null);
        TPeriod infinitePeriod = CreatePeriod(null, null);
        PeriodHistory<TPeriod, T> history = CreatePeriodHistory([period]);

        // Act
        IList<TPeriod> periodsBefore = history.GetPeriodsOverlappingAt(CreatePoint(2017, 5, 1), CreatePoint(2018, 1, 1));
        IList<TPeriod> periodsContaining = history.GetPeriodsOverlappingAt(CreatePoint(2018, 1, 10), CreatePoint(2018, 1, 15));
        IList<TPeriod> periodsContaining2 = history.GetPeriodsOverlappingAt(CreatePoint(2018, 1, 10), infinitePeriod.CoalesceTo);
        IList<TPeriod> periodsOverlappingStart = history.GetPeriodsOverlappingAt(CreatePoint(2017, 12, 10), CreatePoint(2018, 1, 15));
        IList<TPeriod> periodsOverlappingStart2 = history.GetPeriodsOverlappingAt(CreatePoint(2017, 12, 10), infinitePeriod.CoalesceTo);

        // Assert
        Assert.That(periodsBefore, Is.Empty);
        Assert.That(periodsContaining, Has.Count.EqualTo(1));
        Assert.That(periodsContaining.Single(), Is.EqualTo(period));
        Assert.That(periodsContaining2, Has.Count.EqualTo(1));
        Assert.That(periodsContaining2.Single(), Is.EqualTo(period));
        Assert.That(periodsOverlappingStart, Has.Count.EqualTo(1));
        Assert.That(periodsOverlappingStart.Single(), Is.EqualTo(period));
        Assert.That(periodsOverlappingStart2, Has.Count.EqualTo(1));
        Assert.That(periodsOverlappingStart2.Single(), Is.EqualTo(period));
    }

    [TestCaseSource(nameof(CreationCases))]
    public string test_creation(StringArray stringArray)
    {
        // Arrange
        T startDate = CreatePoint(2017, 1, 1);

        List<TPeriod> allPeriods = new ();
        foreach (string periodAsString in stringArray.Strings)
        {
            allPeriods.AddRange(ConvertStringToPeriods(startDate, periodAsString));
        }

        // Act
        PeriodHistory<TPeriod, T> periodHistory = CreatePeriodHistory(allPeriods);
        string actualPeriodHistoryAsString = ConvertPeriodsToString(startDate, periodHistory.Periods);

        // Assert
        return actualPeriodHistoryAsString;
    }

    [TestCaseSource(nameof(IntersectWithCases))]
    public void test_intersect_with(
        string initialPeriodsAsString,
        string intersectPeriodsAsString,
        string expectedPeriodsAsString)
    {
        // Arrange
        T startDate = CreatePoint(2017, 1, 1);

        PeriodHistory<TPeriod, T> initialPeriods = CreatePeriodHistory(ConvertStringToPeriods(startDate, initialPeriodsAsString));
        PeriodHistory<TPeriod, T> intersectPeriods = CreatePeriodHistory(ConvertStringToPeriods(startDate, intersectPeriodsAsString));
        PeriodHistory<TPeriod, T> expectedPeriods = CreatePeriodHistory(ConvertStringToPeriods(startDate, expectedPeriodsAsString));

        // Act
        IEnumerable<IPeriod<T>> actualPeriods = initialPeriods.IntersectWith(intersectPeriods);

        // Assert
        Assert.That(actualPeriods.SetEqual(expectedPeriods.Periods, new PeriodComparer<T>()));
    }

    [TestCaseSource(nameof(ExceptWithCases))]
    public void test_except_with(
        string initialPeriodsAsString,
        string exceptPeriodsAsString,
        string expectedPeriodsAsString)
    {
        // Arrange
        T startDate = CreatePoint(2017, 1, 1);

        PeriodHistory<TPeriod, T> initialPeriods = CreatePeriodHistory(ConvertStringToPeriods(startDate, initialPeriodsAsString));
        PeriodHistory<TPeriod, T> exceptPeriods = CreatePeriodHistory(ConvertStringToPeriods(startDate, exceptPeriodsAsString));
        PeriodHistory<TPeriod, T> expectedPeriods = CreatePeriodHistory(ConvertStringToPeriods(startDate, expectedPeriodsAsString));

        // Act
        IEnumerable<IPeriod<T>> actualPeriods = initialPeriods.ExceptWith(exceptPeriods);

        // Assert
        Assert.That(actualPeriods.SetEqual(expectedPeriods.Periods, new PeriodComparer<T>()));
    }

    [TestCase(
        "XXXX_",
        "_XX__",
        "XXXX_")]
    [TestCase(
        "X_X_X",
        "__XXX",
        "__X_X")]
    [TestCase(
        "X_X_XXX_X.",
        "___XXXXXX_",
        "____XXX_X.")]
    [TestCase(
        "XXXX_XXXX.",
        "____X_____",
        "__________")]
    public void test_get_periods_overlapping_at(string initial, string overlap, string expected)
    {
        // Arrange
        T startDate = CreatePoint(2020, 1, 1);
        PeriodHistory<TPeriod, T> periodHistory = CreatePeriodHistory(ConvertStringToPeriods(startDate, initial));
        TPeriod[] overlapPeriods = ConvertStringToPeriods(startDate, overlap);
        Contract.Assert(overlapPeriods.Length == 1);
        TPeriod overlapPeriod = overlapPeriods[0];

        // Act
        IList<TPeriod> overlappingPeriods = periodHistory.GetPeriodsOverlappingAt(overlapPeriod);

        // Assert
        Assert.That(
            CanonicalizePeriodsString(ConvertPeriodsToString(startDate, overlappingPeriods)),
            Is.EqualTo(CanonicalizePeriodsString(expected)));
    }

    [TestCase("XX_X_.", 0, "XX____")]
    [TestCase("XX_X_.", 1, "XX____")]
    [TestCase("XX_X_.", 2, "___X__")]
    [TestCase("XX_X_.", 3, "___X__")]
    [TestCase("XX_X_.", 4, "_____.")]
    [TestCase("XX_X_.", 5, "_____.")]
    [TestCase("XX_X_.", 6, "_____.")]
    [TestCase("XX_X__", 6, "______")]
    public void test_period_at_or_immediately_after(string initial, int idx, string expected)
    {
        // Arrange
        T startDate = CreatePoint(2020, 1, 1);
        PeriodHistory<TPeriod, T> periodHistory = CreatePeriodHistory(ConvertStringToPeriods(startDate, initial));
        T point = AddToPoint(startDate, idx);

        // Act
        TPeriod? period = periodHistory.GetPeriodAtOrImmediatelyAfter(point);

        // Assert
        Assert.That(
            CanonicalizePeriodsString(ConvertPeriodsToString(startDate, period != null ? [period] : [])),
            Is.EqualTo(CanonicalizePeriodsString(expected)));
    }

    [TestCase("XX_X_.", 0, "XX____")]
    [TestCase("XX_X_.", 1, "XX____")]
    [TestCase("XX_X_.", 2, "XX____")]
    [TestCase("XX_X_.", 3, "___X__")]
    [TestCase("XX_X_.", 4, "___X__")]
    [TestCase("XX_X_.", 5, "_____.")]
    [TestCase("XX_X_.", 6, "_____.")]
    [TestCase("XX_X__", 6, "___X__")]
    [TestCase("__XX__", 1, "______")]
    public void test_period_at_or_immediately_before(string initial, int idx, string expected)
    {
        // Arrange
        T startDate = CreatePoint(2020, 1, 1);
        PeriodHistory<TPeriod, T> periodHistory = CreatePeriodHistory(ConvertStringToPeriods(startDate, initial));
        T point = AddToPoint(startDate, idx);

        // Act
        TPeriod? period = periodHistory.GetPeriodAtOrImmediatelyBefore(point);

        // Assert
        Assert.That(
            CanonicalizePeriodsString(ConvertPeriodsToString(startDate, period != null ? [period] : [])),
            Is.EqualTo(CanonicalizePeriodsString(expected)));
    }

    [TestCase("XX_X__", 0, "XX____")]
    [TestCase("XX_X__", 1, "XX____")]
    [TestCase("XX_X__", 2, "___X__")]
    [TestCase("XX_X__", 3, "___X__")]
    [TestCase("XX_X__", 4, "___X__")]
    [TestCase("XX_X__", 5, "___X__")]
    [TestCase("XX_X__", 6, "___X__")]
    [TestCase("XX_X_.", 3, "___X__")]
    [TestCase("XX_X_.", 4, "_____.")]
    [TestCase("XX_X_.", 5, "_____.")]
    [TestCase("XX_X_.", 6, "_____.")]
    public void test_period_at_or_immediately_after_or_immediately_before(string initial, int idx, string expected)
    {
        // Arrange
        T startDate = CreatePoint(2020, 1, 1);
        PeriodHistory<TPeriod, T> periodHistory = CreatePeriodHistory(ConvertStringToPeriods(startDate, initial));
        T point = AddToPoint(startDate, idx);

        // Act
        TPeriod? period = periodHistory.GetPeriodAtOrImmediatelyAfterOrImmediatelyBefore(point);

        // Assert
        Assert.That(
            CanonicalizePeriodsString(ConvertPeriodsToString(startDate, period != null ? [period] : [])),
            Is.EqualTo(CanonicalizePeriodsString(expected)));
    }

    [TestCase("XX_X__", 0, "XX____")]
    [TestCase("XX_X__", 1, "XX____")]
    [TestCase("XX_X__", 2, "XX____")]
    [TestCase("XX_X__", 3, "___X__")]
    [TestCase("XX_X__", 4, "___X__")]
    [TestCase("XX_X__", 5, "___X__")]
    [TestCase("XX_X__", 6, "___X__")]
    [TestCase("XX_X_.", 3, "___X__")]
    [TestCase("XX_X_.", 4, "___X__")]
    [TestCase("XX_X_.", 5, "_____.")]
    [TestCase("XX_X_.", 6, "_____.")]
    public void test_period_at_or_immediately_before_or_immediately_after(string initial, int idx, string expected)
    {
        // Arrange
        T startDate = CreatePoint(2020, 1, 1);
        PeriodHistory<TPeriod, T> periodHistory = CreatePeriodHistory(ConvertStringToPeriods(startDate, initial));
        T point = AddToPoint(startDate, idx);

        // Act
        TPeriod? period = periodHistory.GetPeriodAtOrImmediatelyBeforeOrImmediatelyAfter(point);

        // Assert
        Assert.That(
            CanonicalizePeriodsString(ConvertPeriodsToString(startDate, period != null ? [period] : [])),
            Is.EqualTo(CanonicalizePeriodsString(expected)));
    }

    [TestCase("XX_X__", 0, true)]
    [TestCase("XX_X__", 1, true)]
    [TestCase("XX_X__", 2, false)]
    [TestCase("XX_X__", 3, true)]
    [TestCase("XX_X__", 4, false)]
    [TestCase("XX_X__", 5, false)]
    [TestCase("XX_X__", 6, false)]
    [TestCase("XX_X_.", 3, true)]
    [TestCase("XX_X_.", 4, false)]
    [TestCase("XX_X_.", 5, true)]
    [TestCase("XX_X_.", 6, true)]
    [TestCase("_XXXX.", 0, false)]
    [TestCase("_XXXX.", 1, true)]
    public void test_has_period_at(string initial, int idx, bool expected)
    {
        // Arrange
        T startDate = CreatePoint(2020, 1, 1);
        PeriodHistory<TPeriod, T> periodHistory = CreatePeriodHistory(ConvertStringToPeriods(startDate, initial));
        T point = AddToPoint(startDate, idx);

        // Act
        bool actual = periodHistory.HasPeriodAt(point);

        // Assert
        Assert.That(actual, Is.EqualTo(expected));
    }

    [TestCase("_____")]
    [TestCase(".____", null, 1)]
    [TestCase("_X_X_", 1, 2, 3, 4)]
    [TestCase("XXXXX", 0, 5)]
    [TestCase("_XXX_", 1, 4)]
    public void test_points(string initial, params int?[] expected)
    {
        // Arrange
        T startDate = CreatePoint(2020, 1, 1);
        PeriodHistory<TPeriod, T> periodHistory = CreatePeriodHistory(ConvertStringToPeriods(startDate, initial));

        // Act
        IList<T?> points = periodHistory.PeriodDateTimes;
        T?[] expectedPoints =
            expected
                .Select(i => i == null ? (T?)null : AddToPoint(startDate, i.Value))
                .ToArray();

        // Assert
        Assert.That(points, Is.EqualTo(expectedPoints));
    }

    [TestCase("_____", null)]
    [TestCase(".____", null)]
    [TestCase("_X_X_", 1)]
    [TestCase("XXXXX", 0)]
    [TestCase("_XXX_", 1)]
    public void test_oldest_point(string initial, int? expected)
    {
        // Arrange
        T startDate = CreatePoint(2020, 1, 1);
        PeriodHistory<TPeriod, T> periodHistory = CreatePeriodHistory(ConvertStringToPeriods(startDate, initial));

        // Act
        T? point = periodHistory.OldestFromDate;
        T? expectedPoint = expected == null ? (T?)null : AddToPoint(startDate, expected.Value);

        // Assert
        Assert.That(point, Is.EqualTo(expectedPoint));
    }
}
