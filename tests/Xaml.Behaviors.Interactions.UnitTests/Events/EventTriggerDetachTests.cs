using System.Collections.Generic;
#if UNO
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Dispatching;
using Xaml.Interactions.Events;
using Xaml.Interactivity;
#else
using Avalonia.Controls;
using Avalonia.Headless.XUnit;
using Avalonia.Threading;
using Avalonia.Xaml.Interactions.Events;
using Avalonia.Xaml.Interactivity;
#endif
using Xunit;

#if UNO
namespace Xaml.Interactions.UnitTests.Events;
#else
namespace Avalonia.Xaml.Interactions.UnitTests.Events;
#endif

public class EventTriggerDetachTests
{
#if UNO
    private sealed class RecordingAction : global::Xaml.Interactivity.Action
#else
    private sealed class RecordingAction : Avalonia.Xaml.Interactivity.Action
#endif
    {
        public List<object?> Parameters { get; } = [];

        public override object? Execute(object? sender, object? parameter)
        {
            Parameters.Add(parameter);
            return null;
        }
    }

    [AvaloniaFact]
    public void Removed_Trigger_Stops_Receiving_Events_While_Element_Stays_Loaded()
    {
        var button = new Button { Content = "b" };
        var other = new TextBox();
        var window = new Window { Content = new StackPanel { Children = { button, other } } };
        window.Show();

        var trigger = new GotFocusEventTrigger();
        var action = new RecordingAction();
        trigger.Actions!.Add(action);
        Interaction.GetBehaviors(button).Add(trigger);

        Interaction.GetBehaviors(button).Remove(trigger);
        button.Focus();
        Dispatcher.UIThread.RunJobs();

        Assert.Empty(action.Parameters);
    }
}
