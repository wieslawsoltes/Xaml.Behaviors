using System;
using System.Net.NetworkInformation;
#if UNO
using Microsoft.UI.Dispatching;
using Xaml.Interactivity;
#else
using Avalonia.Threading;
using Avalonia.Xaml.Interactivity;
#endif

#if UNO
namespace Xaml.Interactions.Network;
#else
namespace Avalonia.Xaml.Interactions.Network;
#endif

/// <summary>
/// A trigger that listens to network availability changes.
/// </summary>
public class NetworkInformationTrigger : Trigger
{
    /// <inheritdoc />
    protected override void OnAttached()
    {
        base.OnAttached();
        NetworkChange.NetworkAvailabilityChanged += OnNetworkAvailabilityChanged;
    }

    /// <inheritdoc />
    protected override void OnDetaching()
    {
        NetworkChange.NetworkAvailabilityChanged -= OnNetworkAvailabilityChanged;
        base.OnDetaching();
    }

    private void OnNetworkAvailabilityChanged(object? sender, NetworkAvailabilityEventArgs e)
    {
        Dispatcher.UIThread.InvokeAsync(() =>
        {
            Interaction.ExecuteActions(AssociatedObject, Actions, e.IsAvailable);
        });
    }
}
