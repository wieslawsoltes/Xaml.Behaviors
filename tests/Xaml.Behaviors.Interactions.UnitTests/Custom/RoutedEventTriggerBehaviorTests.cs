#if UNO
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Dispatching;
using Xaml.Interactions.Custom;
using Xaml.Interactivity;
#else
using Avalonia.Controls;
using Avalonia.Headless.XUnit;
using Avalonia.Interactivity;
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

public class RoutedEventTriggerBehaviorTests
{
#if !UNO
    // Window.WindowClosedEvent is an Avalonia routed event of the top level; WinUI windows raise a CLR Closed event.
    [AvaloniaFact]
    public void WindowClosedEvent_ExecutesBoundCommand()
    {
        var window = new WindowClosedRoutedEventWindow();
        var source = Assert.IsType<WindowClosedBindingSource>(window.DataContext);
        var behavior = Assert.IsType<RoutedEventTriggerBehavior>(
            Assert.Single(Interaction.GetBehaviors(window)));
        var action = new CountingAction();
        behavior.Actions!.Add(action);

        window.Show();
        window.Close();
        Dispatcher.UIThread.RunJobs();

        Assert.Equal(1, action.ExecutionCount);
        Assert.Equal(1, source.CloseCommand.ExecutionCount);
        Assert.Null(behavior.AssociatedObject);

        window.RaiseEvent(new RoutedEventArgs(Window.WindowClosedEvent));

        Assert.Equal(1, action.ExecutionCount);
        Assert.Equal(1, source.CloseCommand.ExecutionCount);
    }
#endif

    [AvaloniaFact]
    public void ControlRoutedEvent_UnsubscribesWhenControlLeavesVisualTree()
    {
#if UNO
        // WinUI has no routed Button.ClickEvent and elements cannot raise routed events: the trigger listens to the
        // pointer pressed event of a border, raised by mouse input.
        var button = new Border { Width = 50, Height = 50, Background = Brushes.Red };
        var panel = new Grid { Children = { button } };
        var window = new Window { Content = panel };
        var behavior = new RoutedEventTriggerBehavior
        {
            RoutedEvent = UIElement.PointerPressedEvent
        };
#else
        var button = new Button();
        var panel = new Panel { Children = { button } };
        var window = new Window { Content = panel };
        var behavior = new RoutedEventTriggerBehavior
        {
            RoutedEvent = Button.ClickEvent
        };
#endif
        var action = new CountingAction();
        behavior.Actions!.Add(action);
        Interaction.GetBehaviors(button).Add(behavior);

        window.Show();
#if UNO
        window.Click(button);
#else
        button.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
#endif

        Assert.Equal(1, action.ExecutionCount);

        panel.Children.Remove(button);
        Dispatcher.UIThread.RunJobs();
#if UNO
        // A detached element receives no input: attach it again and check that the handler is subscribed only once.
        panel.Children.Add(button);
        Dispatcher.UIThread.RunJobs();
        window.Click(button);

        Assert.Equal(2, action.ExecutionCount);
#else
        button.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));

        Assert.Equal(1, action.ExecutionCount);
#endif
    }

#if UNO
    private sealed class CountingAction : Xaml.Interactivity.Action
#else
    private sealed class CountingAction : Avalonia.Xaml.Interactivity.Action
#endif
    {
        public int ExecutionCount { get; private set; }

        public override object? Execute(object? sender, object? parameter)
        {
            ExecutionCount++;
            return null;
        }
    }
}
