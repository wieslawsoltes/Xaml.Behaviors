// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using System;
using Avalonia.Controls;
using Avalonia.Threading;
using Avalonia.Xaml.Interactivity;

namespace Avalonia.Xaml.Interactions.Custom;

/// <summary>
/// Sets focus on the associated control when <see cref="FocusFlag"/> is true.
/// </summary>
public class FocusControlBehavior : AttachedToVisualTreeBehavior<Control>
{
    /// <summary>
    /// Gets or sets a value indicating whether the control should be focused.
    /// </summary>
    public static readonly StyledProperty<bool> FocusFlagProperty =
        AvaloniaProperty.Register<FocusControlBehavior, bool>(nameof(FocusFlag));

    /// <summary>
    /// 
    /// </summary>
    public bool FocusFlag
    {
        get => GetValue(FocusFlagProperty);
        set => SetValue(FocusFlagProperty, value);
    }

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
