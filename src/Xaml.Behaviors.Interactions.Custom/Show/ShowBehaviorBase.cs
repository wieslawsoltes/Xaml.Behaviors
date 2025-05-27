// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Threading;

namespace Avalonia.Xaml.Interactions.Custom;

/// <summary>
/// Base class for behaviors that show a control in response to an event.
/// </summary>
public abstract class ShowBehaviorBase : AttachedToVisualTreeBehavior<Control>
{
    /// <summary>
    /// Identifies the <seealso cref="TargetControl"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<Control?> TargetControlProperty =
        AvaloniaProperty.Register<ShowBehaviorBase, Control?>(nameof(TargetControl));

    /// <summary>
    /// Gets or sets the routing strategy used for the triggering event.
    /// </summary>
    public static readonly StyledProperty<RoutingStrategies> EventRoutingStrategyProperty =
        AvaloniaProperty.Register<ShowBehaviorBase, RoutingStrategies>(nameof(EventRoutingStrategy), RoutingStrategies.Bubble);

    /// <summary>
    /// Gets or sets the target control. This is an avalonia property.
    /// </summary>
    [ResolveByName]
    public Control? TargetControl
    {
        get => GetValue(TargetControlProperty);
        set => SetValue(TargetControlProperty, value);
    }

    /// <summary>
    /// 
    /// </summary>
    public RoutingStrategies EventRoutingStrategy
    {
        get => GetValue(EventRoutingStrategyProperty);
        set => SetValue(EventRoutingStrategyProperty, value);
    }

    /// <summary>
    /// Shows the <see cref="TargetControl"/> when the behavior is triggered.
    /// </summary>
    /// <returns>True if the control was shown; otherwise, false.</returns>
    protected bool Show()
    {
        if (IsEnabled && TargetControl is { IsVisible: false })
        {
            TargetControl.SetCurrentValue(Visual.IsVisibleProperty, true);

            Dispatcher.UIThread.Post(() => TargetControl.Focus());

            return true;
        }

        return false;
    }
}
