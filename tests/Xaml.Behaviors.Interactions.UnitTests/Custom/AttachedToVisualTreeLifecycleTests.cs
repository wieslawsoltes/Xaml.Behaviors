using System;
#if UNO
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Dispatching;
using Xaml.Interactions.Core;
using Xaml.Interactions.Custom;
using Xaml.Interactions.UnitTests.Core;
using Xaml.Interactivity;
#else
using Avalonia.Controls;
using Avalonia.Headless;
using Avalonia.Headless.XUnit;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia.Threading;
using Avalonia.Xaml.Interactions.Core;
using Avalonia.Xaml.Interactions.Custom;
using Avalonia.Xaml.Interactions.UnitTests.Core;
using Avalonia.Xaml.Interactivity;
#endif
using Xunit;

#if UNO
namespace Xaml.Interactions.UnitTests.Custom;
#else
namespace Avalonia.Xaml.Interactions.UnitTests.Custom;
#endif

public class AttachedToVisualTreeLifecycleTests
{
    private sealed class TrackingTrigger : AttachedToVisualTreeTriggerBase<Border>
    {
        public int ActiveSubscriptions { get; private set; }
        public int CreatedSubscriptions { get; private set; }
        public int DisposedSubscriptions { get; private set; }

        protected override IDisposable OnAttachedToVisualTreeOverride()
        {
            ActiveSubscriptions++;
            CreatedSubscriptions++;

            return DisposableAction.Create(() =>
            {
                ActiveSubscriptions--;
                DisposedSubscriptions++;
            });
        }
    }

    private sealed class NonGenericPointerEnteredTrigger : RoutedEventTrigger
    {
        protected override RoutedEvent RoutedEvent => InputElement.PointerEnteredEvent;
    }

    private sealed class TrackingBehavior : AttachedToVisualTreeBehavior<Border>
    {
        public int ActiveSubscriptions { get; private set; }
        public int CreatedSubscriptions { get; private set; }
        public int DisposedSubscriptions { get; private set; }

        protected override IDisposable OnAttachedToVisualTreeOverride()
        {
            ActiveSubscriptions++;
            CreatedSubscriptions++;

            return DisposableAction.Create(() =>
            {
                ActiveSubscriptions--;
                DisposedSubscriptions++;
            });
        }
    }

    [AvaloniaFact]
    public void PointerEnteredTrigger_ExecutesOnceAfterVisualTreeReattach()
    {
        var commandCalls = 0;
        var nonGenericCommandCalls = 0;
        var target = new Border
        {
            Width = 100,
            Height = 100,
            Background = Brushes.Red,
        };
        var outside = new Border
        {
            Width = 100,
            Height = 100,
            Background = Brushes.Blue,
        };
        var panel = new StackPanel
        {
            Children = { target, outside },
        };
        var trigger = new PointerEnteredTrigger();
        var action = new InvokeCommandAction
        {
            Command = new Command(_ => commandCalls++),
        };
        trigger.Actions ??= [];
        trigger.Actions.Add(action);
        var nonGenericTrigger = new NonGenericPointerEnteredTrigger();
        var nonGenericAction = new InvokeCommandAction
        {
            Command = new Command(_ => nonGenericCommandCalls++),
        };
        nonGenericTrigger.Actions ??= [];
        nonGenericTrigger.Actions.Add(nonGenericAction);
        var behaviors = Interaction.GetBehaviors(target);
        behaviors.Add(trigger);
        behaviors.Add(nonGenericTrigger);
        var window = new Window
        {
            Width = 240,
            Height = 240,
            Content = panel,
        };
        window.Show();
        window.CaptureRenderedFrame();

        window.MouseMove(outside, new Point(10, 10));
        window.MouseMove(target, new Point(10, 10));

        Assert.Equal(1, commandCalls);
        Assert.Equal(1, nonGenericCommandCalls);

        panel.Children.Remove(target);
        Dispatcher.UIThread.RunJobs();
        panel.Children.Insert(0, target);
        Dispatcher.UIThread.RunJobs();
        window.CaptureRenderedFrame();

        window.MouseMove(outside, new Point(10, 10));
        var callsBeforeSecondEntry = commandCalls;
        var nonGenericCallsBeforeSecondEntry = nonGenericCommandCalls;
        window.MouseMove(target, new Point(10, 10));

        Assert.Equal(callsBeforeSecondEntry + 1, commandCalls);
        Assert.Equal(nonGenericCallsBeforeSecondEntry + 1, nonGenericCommandCalls);

        Interaction.SetBehaviors(target, null);
        window.MouseMove(outside, new Point(10, 10));
        var callsBeforeDetachedEntry = commandCalls;
        var nonGenericCallsBeforeDetachedEntry = nonGenericCommandCalls;
        window.MouseMove(target, new Point(10, 10));

        Assert.Equal(callsBeforeDetachedEntry, commandCalls);
        Assert.Equal(nonGenericCallsBeforeDetachedEntry, nonGenericCommandCalls);
    }

    [AvaloniaFact]
    public void AttachedToVisualTreeTriggerBase_DisposesForEachVisualTreeLifetime()
    {
        var target = new Border();
#if WINUI
        // WinUI Panel cannot be created directly (Uno Platform and Avalonia allow it).
        var panel = new Grid { Children = { target } };
#else
        var panel = new Panel { Children = { target } };
#endif
        var trigger = new TrackingTrigger();
        Interaction.GetBehaviors(target).Add(trigger);
        var window = new Window { Content = panel };
        window.Show();

        Assert.Equal(1, trigger.ActiveSubscriptions);

        panel.Children.Remove(target);
#if WINUI
        // Native WinUI raises Unloaded for a removed element asynchronously (Uno Platform raises it at once).
        Dispatcher.UIThread.RunJobs();
#endif

        Assert.Equal(0, trigger.ActiveSubscriptions);
        Assert.Equal(1, trigger.DisposedSubscriptions);

        panel.Children.Add(target);
#if UNO
        // WinUI raises Loaded for an element added to a loaded tree with the next layout pass.
        Dispatcher.UIThread.RunJobs();
#endif

        Assert.Equal(1, trigger.ActiveSubscriptions);
        Assert.Equal(2, trigger.CreatedSubscriptions);

        panel.Children.Remove(target);
#if WINUI
        // Native WinUI raises Unloaded for a removed element asynchronously (Uno Platform raises it at once).
        Dispatcher.UIThread.RunJobs();
#endif

        Assert.Equal(0, trigger.ActiveSubscriptions);
        Assert.Equal(2, trigger.DisposedSubscriptions);
    }

    [AvaloniaFact]
    public void AttachedToVisualTreeBehavior_DisposesForEachVisualTreeLifetime()
    {
        var target = new Border();
#if WINUI
        // WinUI Panel cannot be created directly (Uno Platform and Avalonia allow it).
        var panel = new Grid { Children = { target } };
#else
        var panel = new Panel { Children = { target } };
#endif
        var behavior = new TrackingBehavior();
        Interaction.GetBehaviors(target).Add(behavior);
        var window = new Window { Content = panel };
        window.Show();

        Assert.Equal(1, behavior.ActiveSubscriptions);

        panel.Children.Remove(target);
#if WINUI
        // Native WinUI raises Unloaded for a removed element asynchronously (Uno Platform raises it at once).
        Dispatcher.UIThread.RunJobs();
#endif

        Assert.Equal(0, behavior.ActiveSubscriptions);
        Assert.Equal(1, behavior.DisposedSubscriptions);

        panel.Children.Add(target);
#if UNO
        // WinUI raises Loaded for an element added to a loaded tree with the next layout pass.
        Dispatcher.UIThread.RunJobs();
#endif

        Assert.Equal(1, behavior.ActiveSubscriptions);
        Assert.Equal(2, behavior.CreatedSubscriptions);

        panel.Children.Remove(target);
#if WINUI
        // Native WinUI raises Unloaded for a removed element asynchronously (Uno Platform raises it at once).
        Dispatcher.UIThread.RunJobs();
#endif

        Assert.Equal(0, behavior.ActiveSubscriptions);
        Assert.Equal(2, behavior.DisposedSubscriptions);
    }
}
