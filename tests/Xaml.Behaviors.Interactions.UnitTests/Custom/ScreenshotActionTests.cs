using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Headless.XUnit;
using Avalonia.Media;
using Avalonia.Threading;
using Avalonia.Xaml.Interactions.Custom;
using Xunit;

namespace Avalonia.Xaml.Interactions.UnitTests.Custom;

// Avalonia storage providers and files are not client implementable ([NotClientImplementable]), so the save path is
// covered by the Uno tests (tests/Uno/Xaml.Behaviors.Interactions.Custom.General.UnitTests/ScreenshotActionTests.cs),
// which use a fake provider.
public class ScreenshotActionTests
{
    [AvaloniaFact]
    public async Task Execute_Starts_A_Capture_For_A_Control_In_A_Top_Level()
    {
        var target = new Border { Width = 40, Height = 30, Background = Brushes.Red };
        var window = new Window { Width = 200, Height = 200, Content = target };
        window.Show();

        var action = new ScreenshotAction { StorageProvider = window.StorageProvider };

        Assert.Same(window.StorageProvider, action.StorageProvider);
        Assert.True((bool)action.Execute(target, null));

        await Task.Yield();
        Dispatcher.UIThread.RunJobs();
        window.Close();
    }

    [AvaloniaFact]
    public void Execute_Returns_False_Without_A_Target_In_A_Top_Level()
    {
        Assert.Null(new ScreenshotAction().StorageProvider);
        Assert.False((bool)new ScreenshotAction().Execute(null, null));
        Assert.False((bool)new ScreenshotAction().Execute(new Border(), null));
        Assert.False((bool)new ScreenshotAction { IsEnabled = false }.Execute(new Border(), null));
    }
}
