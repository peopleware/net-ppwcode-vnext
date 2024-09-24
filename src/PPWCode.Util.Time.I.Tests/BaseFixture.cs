using NUnit.Framework;

namespace PPWCode.Util.Time.I.Tests;

[TestFixture]
[Parallelizable(ParallelScope.Fixtures)]
public abstract class BaseFixture
{
    [SetUp]
    public void Setup()
    {
        OnSetup();
    }

    [TearDown]
    public void Teardown()
    {
        OnTeardown();
    }

    [OneTimeSetUp]
    public void FixtureSetup()
    {
        OnOneTimeSetup();
    }

    [OneTimeTearDown]
    public void FixtureTeardown()
    {
        OnOneTimeTeardown();
    }

    protected virtual void OnOneTimeSetup()
    {
    }

    protected virtual void OnOneTimeTeardown()
    {
    }

    protected virtual void OnSetup()
    {
    }

    protected virtual void OnTeardown()
    {
    }
}
