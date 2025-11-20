using System;
using System.Net.NetworkInformation;
using Avalonia.Controls;
using Avalonia.Threading;
using Avalonia.Xaml.Interactivity;

namespace Avalonia.Xaml.Interactions.Custom;

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
public class NetworkStatusTrigger : StyledElementTrigger<Control>
{
    /// <summary>
    /// Identifies the <seealso cref="Status"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<NetworkStatus> StatusProperty =
        AvaloniaProperty.Register<NetworkStatusTrigger, NetworkStatus>(nameof(Status), NetworkStatus.Any);

    /// <summary>
    /// Gets or sets the network status to listen for.
    /// </summary>
    public NetworkStatus Status
    {
        get => GetValue(StatusProperty);
        set => SetValue(StatusProperty, value);
    }

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
