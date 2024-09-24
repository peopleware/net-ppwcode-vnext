namespace PPWCode.Util.Time.I.Tests;

public class StringArray
{
    public string?[] Strings { get; }

    public StringArray(IEnumerable<string?> strings)
    {
        Strings = strings.ToArray();
    }
}
