namespace PPWCode.Util.Time.I.Tests;

public class DateOnlyPeriodTests : PeriodTests<DateOnlyPeriod, DateOnly>
{
    /// <inheritdoc />
    protected override DateOnlyPeriod Create(DateOnly? from, DateOnly? to)
        => new (from, to);

    /// <inheritdoc />
    protected override DateOnly? ConvertFromString(string? value)
        => string.IsNullOrWhiteSpace(value) ? null : DateOnly.Parse(value);

    /// <inheritdoc />
    protected override string? ConvertToString(DateOnly? value)
        => value is null ? "null" : $"{value.Value:yyyy-MM-dd}";
}
