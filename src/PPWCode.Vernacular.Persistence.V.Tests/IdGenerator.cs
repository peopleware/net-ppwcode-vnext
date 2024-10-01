namespace PPWCode.Vernacular.Persistence.V.Tests;

public static class IdGenerator
{
    private static int _nextId = 1000000;

    public static int NextId
        => Interlocked.Increment(ref _nextId);
}
