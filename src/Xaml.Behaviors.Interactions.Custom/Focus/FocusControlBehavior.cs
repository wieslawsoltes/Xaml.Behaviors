// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using System;
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
/// Sets focus on the associated control when <see cref="FocusFlag"/> is true.
/// </summary>
public partial class FocusControlBehavior : AttachedToVisualTreeBehavior<Control>
{

    /// <summary>
    /// 
    /// </summary>
    [StyledProperty]
    public partial bool FocusFlag { get; set; }

    /// <inheritdoc />
    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);

        if (change.Property == FocusFlagProperty)
        {
            var focusFlag = change.GetNewValue<bool>();
            if (focusFlag && IsEnabled)
            {
                Execute();
            }
        }
    }

    /// <summary>
    /// Invoked when the behavior is attached to the visual tree.
    /// </summary>
    protected override System.IDisposable OnAttachedToVisualTreeOverride()
    {
        if (FocusFlag && IsEnabled)
        {
            Execute();
        }
        
        return DisposableAction.Empty;
    }

    private void Execute()
    {
        Dispatcher.UIThread.Post(() => AssociatedObject?.Focus());
    }
}
