using System.Collections;
using System.Diagnostics.CodeAnalysis;

using NUnit.Framework;

namespace PPWCode.Util.Time.I.Tests;

[TestFixture]
[SuppressMessage("ReSharper", "InconsistentNaming", Justification = "Tests")]
public abstract class PeriodMultiHistoryTests<TPeriod, T> : BasePeriodTests<TPeriod, T>
    where TPeriod : class, IPeriod<T>
    where T : struct, IComparable<T>, IEquatable<T>
{
    public static IEnumerable EmptyPeriodMultiHistoryCases
    {
        get
        {
            yield return
                new TestCaseData(
                        new Func<PeriodMultiHistory<TPeriod, T>, T, IEnumerable<IPeriod<T>>>((mh, d) => mh.GetPeriodsAt(d)))
                    .SetName("empty_periods_GetPeriodsAt");
            yield return
                new TestCaseData(
                        new Func<PeriodMultiHistory<TPeriod, T>, T, IEnumerable<IPeriod<T>>>((mh, _) => mh.GetPeriodsOverlappingAt(null, null)))
                    .SetName("empty_periods_GetPeriodsOverlappingAt");
            yield return
                new TestCaseData(
                        new Func<PeriodMultiHistory<TPeriod, T>, T, IEnumerable<IPeriod<T>>>((mh, _) => mh.GetOptimalCoveringPeriods()))
                    .SetName("empty_periods_GetOptimalCoveringPeriods");
        }
    }

    public static IEnumerable CoveringPeriodMultiHistoryCases
    {
        get
        {
            yield return
                new TestCaseData(
                        new StringArray(
                        [
                            "__XXXXXX."
                        ]))
                    .Returns("__.");
            yield return
                new TestCaseData(
                        new StringArray(
                        [
                            "__XXXXXX_"
                        ]))
                    .Returns("__XXXXXX");
            yield return
                new TestCaseData(
                        new StringArray(
                        [
                            "__XXX_XXX_XX___X__XX____XXXX_____XX_XXX_XXX.",
                            "____XXXX_______XXXXXX______________X____XX__",
                            "____________________________XX______________"
                        ]))
                    .Returns("__XXXXXXX_XX___XXXXXX___XXXXXX___XXXXXX_.");
            yield return
                new TestCaseData(
                        new StringArray(
                        [
                            "__XXX_XXX_XX___X__XX____XXXX_____XX_XXX_XXX_",
                            "____XXXX_______XXXXXX______________X____XX__",
                            "____________________________XX______________"
                        ]))
                    .Returns("__XXXXXXX_XX___XXXXXX___XXXXXX___XXXXXX_XXX");
            yield return
                new TestCaseData(
                        new StringArray(
                        [
                            "_XXXX__X_X_X_X____________XXXXX_______________________________________XXX_",
                            "________X_X_X_____________XXXXX______________________________________XXX__",
                            "__XX____X_X_X___________XXXXXXXXXX________________________XX___________XX.",
                            "________________________XX_XX___XX_______XX___________________XX________X.",
                            "________________________XX_XX___XX_________XXX____________________________",
                            "________________________XX_XX_X_XX____________XXX_______XXXX___XXX______X_"
                        ]))
                    .Returns("_XXXX__XXXXXXX__________XXXXXXXXXX_______XXXXXXXX_______XXXX__XXXX___.");
        }
    }

    protected abstract PeriodMultiHistory<TPeriod, T> CreateMultiPeriodHistory(IEnumerable<IPeriod<T>> periods);

    [Test]
    [TestCaseSource(nameof(EmptyPeriodMultiHistoryCases))]
    public void test_can_handle_empty_periods(Func<PeriodMultiHistory<TPeriod, T>, T, IEnumerable<IPeriod<T>>> lambda)
    {
        // Arrange
        T startDate = CreatePoint(2017, 1, 1);
        PeriodMultiHistory<TPeriod, T> periodMultiHistory = CreateMultiPeriodHistory([]);

        // Act
        IEnumerable<IPeriod<T>> periods = lambda(periodMultiHistory, startDate);

        // Assert
        Assert.That(periods, Is.Not.Null);
        Assert.That(periods, Is.Empty);
    }

    [Test]
    [TestCaseSource(nameof(CoveringPeriodMultiHistoryCases))]
    public string? test_covering_periods(StringArray stringArray)
    {
        // Arrange
        T startDate = CreatePoint(2017, 7, 1);

        List<TPeriod> allPeriods = new ();
        foreach (string? periodAsString in stringArray.Strings)
        {
            allPeriods.AddRange(ConvertStringToPeriods(startDate, periodAsString));
        }

        // Act
        PeriodMultiHistory<TPeriod, T> periodMultiHistory = CreateMultiPeriodHistory(allPeriods);
        IEnumerable<IPeriod<T>> optimalPeriods = periodMultiHistory.GetOptimalCoveringPeriods();
        string actualPeriodHistoryAsString = ConvertPeriodsToString(startDate, optimalPeriods);

        // Assert
        return actualPeriodHistoryAsString;
    }
}
