using NUnit.Framework;

namespace PPWCode.Util.DI.I.Tests;

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
    public void TearDown()
    {
        OnTearDown();
    }

    [OneTimeSetUp]
    public void OneTimeSetup()
    {
        OnOneTimeSetup();
    }

    [OneTimeTearDown]
    public void OneTimeTearDown()
    {
        OnOneTimeTearDown();
    }

    protected virtual void OnOneTimeTearDown()
    {
    }

    protected virtual void OnOneTimeSetup()
    {
    }

    protected virtual void OnSetup()
    {
    }

    protected virtual void OnTearDown()
    {
    }
}
