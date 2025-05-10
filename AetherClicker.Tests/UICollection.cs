using Xunit;
using System.Windows.Threading;
using System.Threading;

namespace AetherClicker.Tests;

[CollectionDefinition("UI")]
public class UICollection : ICollectionFixture<UITestFixture>
{
    // This class has no code, and is never created. Its purpose is simply
    // to be the place to apply [CollectionDefinition] and all the
    // ICollectionFixture<> interfaces.
}

public class UITestFixture : IDisposable
{
    private Dispatcher? _dispatcher;
    private readonly ManualResetEvent _dispatcherReady = new(false);

    public UITestFixture()
    {
        // Create a new STA thread for UI tests
        var thread = new Thread(() =>
        {
            _dispatcher = Dispatcher.CurrentDispatcher;
            _dispatcherReady.Set();
            Dispatcher.Run();
        });
        thread.SetApartmentState(ApartmentState.STA);
        thread.Start();
        _dispatcherReady.WaitOne();
    }

    public void Dispose()
    {
        _dispatcher?.InvokeShutdown();
    }
} 