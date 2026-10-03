#if UNO
using Microsoft.UI.Xaml.Controls;
using Xaml.Interactions.Custom;
using Xaml.Interactivity;
#else
using Avalonia.Controls;
using Avalonia.Headless.XUnit;
using Avalonia.Threading;
using Avalonia.Xaml.Interactions.Custom;
using Avalonia.Xaml.Interactivity;
#endif
using Xunit;

#if UNO
namespace Xaml.Interactions.UnitTests.Custom;
#else
namespace Avalonia.Xaml.Interactions.UnitTests.Custom;
#endif

public class BindTagToVisualRootDataContextBehaviorTests
{
    [AvaloniaFact]
    public void Binds_Tag_When_Attached_Before_The_Element_Is_Shown()
    {
        // Behaviors declared in XAML are attached before their element has a visual root.
        var border = new Border();
        Interaction.GetBehaviors(border).Add(new BindTagToVisualRootDataContextBehavior());
        var window = new Window { DataContext = "context", Content = border };

        window.Show();
        Dispatcher.UIThread.RunJobs();

        Assert.Equal("context", border.Tag);

        window.DataContext = "changed";
        Dispatcher.UIThread.RunJobs();

        Assert.Equal("changed", border.Tag);
        window.Close();
    }
}
