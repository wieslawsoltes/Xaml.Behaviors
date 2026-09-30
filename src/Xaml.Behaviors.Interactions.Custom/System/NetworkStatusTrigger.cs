using System;
using System.Net.NetworkInformation;
#if UNO
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Dispatching;
using Xaml.Interactivity;
#else
using Avalonia.Controls;
using Avalonia.Threading;
using Avalonia.Xaml.Interactivity;
#endif

#if UNO
namespace Xaml.Interactions.Custom;
#else
namespace Avalonia.Xaml.Interactions.Custom;
#endif

/// <summary>
/// Defines the network status to listen for.
/// </summary>
public enum NetworkStatus
{
    /// <summary>
    /// The network is available (Online).
    /// </summary>
    Online,

    /// <summary>
    /// The network is not available (Offline).
    /// </summary>
    Offline,
    
    /// <summary>
    /// Any change in network status.
    /// </summary>
    Any
}

/// <summary>
/// A trigger that fires when the network status changes.
/// </summary>
public partial class NetworkStatusTrigger : StyledElementTrigger<Control>
{

    /// <summary>
    /// Gets or sets the network status to listen for.
    /// </summary>
    [StyledProperty(DefaultValue = NetworkStatus.Any)]
    public partial NetworkStatus Status { get; set; }

    /// <inheritdoc />
    protected override void OnAttachedToVisualTree()
    {
        base.OnAttachedToVisualTree();
        NetworkChange.NetworkAvailabilityChanged += OnNetworkAvailabilityChanged;
    }

    /// <inheritdoc />
    protected override void OnDetachedFromVisualTree()
    {
        base.OnDetachedFromVisualTree();
        NetworkChange.NetworkAvailabilityChanged -= OnNetworkAvailabilityChanged;
    }

    private void OnNetworkAvailabilityChanged(object? sender, NetworkAvailabilityEventArgs e)
    {
        Dispatcher.UIThread.Post(() =>
        {
            var isAvailable = e.IsAvailable;
            var status = Status;

            if (status == NetworkStatus.Any)
            {
                Interaction.ExecuteActions(AssociatedObject, Actions, isAvailable);
            }
            else if (status == NetworkStatus.Online && isAvailable)
            {
                Interaction.ExecuteActions(AssociatedObject, Actions, isAvailable);
            }
            else if (status == NetworkStatus.Offline && !isAvailable)
            {
                Interaction.ExecuteActions(AssociatedObject, Actions, isAvailable);
            }
        });
    }
}
