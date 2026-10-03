#if !UNO
// ScrollGestureEndedEventBehavior is not ported to Uno Platform (WinUI has no scroll gesture events).
using Avalonia.Controls;
using Avalonia.Headless.XUnit;
using Avalonia.Input;
using Avalonia.Xaml.Interactions.Events;
using Avalonia.Xaml.Interactivity;
using Xunit;

namespace Avalonia.Xaml.Interactions.UnitTests.Events;

public class ScrollGestureEndedEventBehaviorTests
{
    private sealed class RecordingBehavior : ScrollGestureEndedEventBehavior
    {
        public ScrollGestureEndedEventArgs? Received { get; private set; }

        protected override void OnScrollGestureEnded(object? sender, ScrollGestureEndedEventArgs e)
        {
            Received = e;
        }
    }

    [AvaloniaFact]
    public void ScrollGestureEndedEvent_Is_Delivered_With_Its_Own_Arguments()
    {
        var target = new Border();
        var behavior = new RecordingBehavior();
        Interaction.GetBehaviors(target).Add(behavior);
        var window = new Window { Content = target };
        window.Show();

        var args = new ScrollGestureEndedEventArgs(1);
        target.RaiseEvent(args);

        Assert.Same(args, behavior.Received);
        window.Close();
    }

    [AvaloniaFact]
    public void ScrollGestureEndedEvent_Is_Not_Delivered_After_Detaching()
    {
        var target = new Border();
        var behavior = new RecordingBehavior();
        Interaction.GetBehaviors(target).Add(behavior);
        var window = new Window { Content = target };
        window.Show();

        Interaction.GetBehaviors(target).Remove(behavior);
        target.RaiseEvent(new ScrollGestureEndedEventArgs(1));

        Assert.Null(behavior.Received);
        window.Close();
    }
}
#endif
