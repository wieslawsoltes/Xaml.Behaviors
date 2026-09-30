// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
#if UNO
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Dispatching;
#else
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Threading;
#endif

#if UNO
namespace Xaml.Interactions.Custom;
#else
namespace Avalonia.Xaml.Interactions.Custom;
#endif

/// <summary>
/// Base class for behaviors that show a control in response to an event.
/// </summary>
public abstract partial class ShowBehaviorBase : AttachedToVisualTreeBehavior<Control>
{

    /// <summary>
    /// Gets or sets the target control. This is an avalonia property.
    /// </summary>
    [StyledProperty(ResolveByName = true)]
    public partial Control? TargetControl { get; set; }

    /// <summary>
    /// 
    /// </summary>
    [StyledProperty(DefaultValue = RoutingStrategies.Bubble)]
    public partial RoutingStrategies EventRoutingStrategy { get; set; }

    /// <summary>
    /// Shows the <see cref="TargetControl"/> when the behavior is triggered.
    /// </summary>
    /// <returns>True if the control was shown; otherwise, false.</returns>
    protected bool Show()
    {
        if (IsEnabled && TargetControl is { IsVisible: false })
        {
#if UNO
            TargetControl.IsVisible = true;
#else
            TargetControl.SetCurrentValue(Visual.IsVisibleProperty, true);
#endif

            Dispatcher.UIThread.Post(() => TargetControl.Focus());

            return true;
        }

        return false;
    }
}
