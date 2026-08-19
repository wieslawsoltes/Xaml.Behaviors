using Avalonia.Controls;
using Avalonia.Headless.XUnit;
using Avalonia.Interactivity;
using Avalonia.Threading;
using Avalonia.Xaml.Interactions.Custom;
using Avalonia.Xaml.Interactivity;
using Xunit;

namespace Avalonia.Xaml.Interactions.UnitTests.Custom;

public class RoutedEventTriggerBehaviorTests
{
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

    [AvaloniaFact]
    public void ControlRoutedEvent_UnsubscribesWhenControlLeavesVisualTree()
    {
        var button = new Button();
        var panel = new Panel { Children = { button } };
        var window = new Window { Content = panel };
        var behavior = new RoutedEventTriggerBehavior
        {
            RoutedEvent = Button.ClickEvent
        };
        var action = new CountingAction();
        behavior.Actions!.Add(action);
        Interaction.GetBehaviors(button).Add(behavior);

        window.Show();
        button.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));

        Assert.Equal(1, action.ExecutionCount);

        panel.Children.Remove(button);
        Dispatcher.UIThread.RunJobs();
        button.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));

        Assert.Equal(1, action.ExecutionCount);
    }

    private sealed class CountingAction : Avalonia.Xaml.Interactivity.Action
    {
        public int ExecutionCount { get; private set; }

        public override object? Execute(object? sender, object? parameter)
        {
            ExecutionCount++;
            return null;
        }
    }
}
