#if UNO
using Microsoft.UI.Dispatching;
#else
using Avalonia.Headless;
using Avalonia.Headless.XUnit;
using Avalonia.Threading;
#endif
using Xunit;

#if UNO
namespace Xaml.Interactions.UnitTests.Custom;
#else
namespace Avalonia.Xaml.Interactions.UnitTests.Custom;
#endif

public class DetachedFromVisualTreeTriggerTests
{
    [AvaloniaFact]
    public void SwitchingTabs_ExecutesBoundDetachedCommand()
    {
        var window = new DetachedFromVisualTreeTrigger001();
        var source = Assert.IsType<DetachedTriggerBindingSource>(window.DataContext);

        window.Show();
        window.CaptureRenderedFrame();

        window.TargetTabs.SelectedIndex = 1;
        Dispatcher.UIThread.RunJobs();
        window.CaptureRenderedFrame();

        Assert.Equal(1, source.DetachedCount);
    }
}
