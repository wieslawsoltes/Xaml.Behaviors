using Avalonia.Headless;
using Avalonia.Headless.XUnit;
using Avalonia.Threading;
using Xunit;

namespace Avalonia.Xaml.Interactions.UnitTests.Custom;

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
