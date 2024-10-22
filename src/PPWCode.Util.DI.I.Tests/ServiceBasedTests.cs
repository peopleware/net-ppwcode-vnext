using Microsoft.Extensions.DependencyInjection;

namespace PPWCode.Util.DI.I.Tests;

public abstract class ServiceBasedTests : BaseFixture
{
    protected ServiceCollection ServiceCollection { get; } = new ();
    private IServiceProvider? _serviceProvider;

    protected IServiceProvider ServiceProvider
        => _serviceProvider ??= ServiceCollection.BuildServiceProvider();

    /// <inheritdoc />
    protected override void OnTearDown()
    {
        _serviceProvider = null;
        ServiceCollection.Clear();

        base.OnTearDown();
    }
}
