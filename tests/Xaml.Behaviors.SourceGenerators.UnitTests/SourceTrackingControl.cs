using System;
using Avalonia.Controls;

namespace Avalonia.Xaml.Behaviors.SourceGenerators.UnitTests;

public class SourceTrackingControl : Control
{
    private EventHandler? _sourceEvent;

    public int SubscriptionCount { get; private set; }

    public event EventHandler? SourceEvent
    {
        add
        {
            _sourceEvent += value;
            SubscriptionCount = _sourceEvent?.GetInvocationList().Length ?? 0;
        }
        remove
        {
            _sourceEvent -= value;
            SubscriptionCount = _sourceEvent?.GetInvocationList().Length ?? 0;
        }
    }

    public void Raise()
    {
        _sourceEvent?.Invoke(this, EventArgs.Empty);
    }
}
