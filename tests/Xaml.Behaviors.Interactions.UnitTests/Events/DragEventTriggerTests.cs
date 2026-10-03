using System.Collections.Generic;
using Avalonia.Controls;
using Avalonia.Headless.XUnit;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Xaml.Interactions.DragAndDrop;
using Avalonia.Xaml.Interactions.Events;
using Avalonia.Xaml.Interactions.UnitTests.DragAndDrop;
using Avalonia.Xaml.Interactivity;
using Xunit;

namespace Avalonia.Xaml.Interactions.UnitTests.Events;

/// <summary>
/// Regression tests for https://github.com/wieslawsoltes/Xaml.Behaviors/issues/381: the drag event triggers observe
/// the drag events that a drop handler on the same element handles.
/// </summary>
public class DragEventTriggerTests
{
    private sealed class RecordingAction : Avalonia.Xaml.Interactivity.Action
    {
        public List<object?> Parameters { get; } = [];

        public override object? Execute(object? sender, object? parameter)
        {
            Parameters.Add(parameter);
            return null;
        }
    }

    private static (Border Target, RecordingAction Action) CreateTarget(InteractiveTriggerBase trigger, System.Action<Border>? addDropHandler = null)
    {
        var target = new Border { Width = 100, Height = 100 };
        addDropHandler?.Invoke(target);
        var behaviors = Interaction.GetBehaviors(target);
        behaviors.Add(new ContextDropBehavior { Handler = new TestDropHandler() });
        var action = new RecordingAction();
        trigger.Actions!.Add(action);
        behaviors.Add(trigger);

        var window = new Window { Content = target };
        window.Show();
        return (target, action);
    }

    private static DragEventArgs Raise(Border target, RoutedEvent<DragEventArgs> routedEvent)
    {
        var args = new DragEventArgs(routedEvent, new DataTransfer(), target, new Point(10, 10), KeyModifiers.None);
        target.RaiseEvent(args);
        return args;
    }

    [AvaloniaFact]
    public void DragEnterEventTrigger_Fires_When_Drop_Handler_Handles_The_Event()
    {
        var (target, action) = CreateTarget(new DragEnterEventTrigger());

        var args = Raise(target, DragDrop.DragEnterEvent);

        Assert.True(args.Handled);
        Assert.Same(args, Assert.Single(action.Parameters));
    }

    [AvaloniaFact]
    public void DragOverEventTrigger_Fires_When_Drop_Handler_Handles_The_Event()
    {
        var (target, action) = CreateTarget(new DragOverEventTrigger());

        var args = Raise(target, DragDrop.DragOverEvent);

        Assert.True(args.Handled);
        Assert.Same(args, Assert.Single(action.Parameters));
    }

    [AvaloniaFact]
    public void DropEventTrigger_Fires_When_Drop_Handler_Handles_The_Event()
    {
        var (target, action) = CreateTarget(new DropEventTrigger());

        var args = Raise(target, DragDrop.DropEvent);

        Assert.True(args.Handled);
        Assert.Same(args, Assert.Single(action.Parameters));
    }

    [AvaloniaFact]
    public void DragLeaveEventTrigger_Fires_When_The_Event_Is_Handled()
    {
        // The drop handlers do not handle DragLeave: a handler registered before the trigger does.
        var (target, action) = CreateTarget(
            new DragLeaveEventTrigger(),
            static border => border.AddHandler(DragDrop.DragLeaveEvent, static (_, e) => e.Handled = true));

        var args = Raise(target, DragDrop.DragLeaveEvent);

        Assert.True(args.Handled);
        Assert.Same(args, Assert.Single(action.Parameters));
    }
}
