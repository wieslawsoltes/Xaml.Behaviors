// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
#if UNO
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Xaml.Interactivity;
#else
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Xaml.Interactivity;
#endif

#if UNO
namespace Xaml.Interactions.Custom;
#else
namespace Avalonia.Xaml.Interactions.Custom;
#endif

/// <summary>
/// A behavior that listens for a <see cref="RoutedEvent"/> event on its source and executes its actions when that event is fired.
/// </summary>
public partial class RoutedEventTriggerBehavior : StyledElementTrigger<Interactive>
{

    private bool _isInitialized;
    private bool _isAttached;

    /// <summary>
    /// Gets or sets routing event to listen for. This is an avalonia property.
    /// </summary>
    [StyledProperty]
    public partial RoutedEvent? RoutedEvent { get; set; }

    /// <summary>
    /// Gets or sets the routing event <see cref="RoutingStrategies"/>. This is an avalonia property.
    /// </summary>
    [StyledProperty(DefaultValue = RoutingStrategies.Direct | RoutingStrategies.Bubble)]
    public partial RoutingStrategies RoutingStrategies { get; set; }

    /// <summary>
    /// Gets or sets the source object from which this behavior listens for events.
    /// If <seealso cref="SourceInteractive"/> is not set, the source will default to <seealso cref="IBehavior.AssociatedObject"/>. This is an avalonia property.
    /// </summary>
    [StyledProperty(ResolveByName = true)]
    public partial Interactive? SourceInteractive { get; set; }

    /// <inheritdoc />
    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
                
        if (change.Property == RoutedEventProperty)
        {
            OnValueChanged(change);
        }

        if (change.Property == RoutingStrategiesProperty)
        {
            OnValueChanged(change);
        }

        if (change.Property == SourceInteractiveProperty)
        {
            OnValueChanged(change);
        }
    }

    private void OnValueChanged(AvaloniaPropertyChangedEventArgs args)
    {
        // Property changes of this behavior are always raised on this instance.
        if (AssociatedObject is null)
        {
            return;
        }

        if (_isInitialized && _isAttached)
        {
            RemoveHandler();
            AddHandler();
        }
    }

    /// <inheritdoc />
    protected override void OnAttachedToVisualTree()
    {
        _isAttached = true;
        AddHandler();
    }

    /// <inheritdoc />
    protected override void OnDetachedFromVisualTree()
    {
        _isAttached = false;

#if UNO
        // WinUI has no top level element that stays attached while its content is unloaded.
        RemoveHandler();
#else
        if (AssociatedObject is not TopLevel || ComputeResolvedSourceInteractive() is not TopLevel)
        {
            RemoveHandler();
        }
#endif
    }

    /// <inheritdoc />
    protected override void OnDetaching()
    {
        _isAttached = false;
        RemoveHandler();
        base.OnDetaching();
    }

    private void AddHandler()
    {
        if (_isInitialized)
        {
            return;
        }

        var interactive = ComputeResolvedSourceInteractive();
        if (interactive is not null && RoutedEvent is not null)
        {
            interactive.AddHandler(RoutedEvent, Handler, RoutingStrategies);
            _isInitialized = true;
        }
    }

    private void RemoveHandler()
    {
        var interactive = ComputeResolvedSourceInteractive();
        if (interactive is not null && RoutedEvent is not null && _isInitialized)
        {
            interactive.RemoveRoutedEventHandler(RoutedEvent, Handler);
            _isInitialized = false;
        }
    }

    private Interactive? ComputeResolvedSourceInteractive()
    {
        return GetValue(SourceInteractiveProperty) is not null ? SourceInteractive : AssociatedObject;
    }

    private void Handler(object? sender, RoutedEventArgs e)
    {
        Execute(e);

        if (!_isAttached)
        {
            RemoveHandler();
        }
    }

    private void Execute(object? parameter)
    {
        if (!IsEnabled)
        {
            return;
        }

        var interactive = ComputeResolvedSourceInteractive();
        if (interactive is not null)
        {
            Interaction.ExecuteActions(interactive, Actions, parameter);
        }
    }
}
