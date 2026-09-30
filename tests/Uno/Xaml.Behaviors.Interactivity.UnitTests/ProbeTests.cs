using System;
using System.Threading.Tasks;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Uno.UI.Hosting;
using Xunit;

public class ProbeTests
{
    [Fact]
    public async Task Loaded_Fires()
    {
        var tcs = new TaskCompletionSource<Window>();
        var thread = new System.Threading.Thread(() =>
        {
            var host = UnoPlatformHostBuilder.Create()
                .App(() => new ProbeApp(tcs))
                .UseHeadless()
                .Build();
            host.Run();
        }) { IsBackground = true };
        thread.Start();
        var window = await tcs.Task.WaitAsync(TimeSpan.FromSeconds(20));
        var loaded = new TaskCompletionSource<bool>();
        window.DispatcherQueue.TryEnqueue(() =>
        {
            var b = new Border();
            b.Loaded += (s, e) => loaded.TrySetResult(true);
            ((Grid)window.Content).Children.Add(b);
        });
        Assert.True(await loaded.Task.WaitAsync(TimeSpan.FromSeconds(10)));
    }
}

public class ProbeApp : Application
{
    private readonly TaskCompletionSource<Window> _tcs;
    public ProbeApp(TaskCompletionSource<Window> tcs) => _tcs = tcs;
    protected override void OnLaunched(LaunchActivatedEventArgs args)
    {
        var w = new Window();
        w.Content = new Grid();
        w.Activate();
        _tcs.TrySetResult(w);
    }
}
