using System;
#if UNO
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
#else
using Avalonia.Controls;
#endif

#if UNO
namespace Xaml.Behaviors.SourceGenerators.UnitTests;
#else
namespace Avalonia.Xaml.Behaviors.SourceGenerators.UnitTests;
#endif

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
