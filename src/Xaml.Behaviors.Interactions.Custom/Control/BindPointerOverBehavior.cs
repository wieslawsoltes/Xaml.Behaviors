// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using System;
#if UNO
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Xaml.Interactivity;
#else
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Input;
using Avalonia.Xaml.Interactivity;
#endif

#if UNO
namespace Xaml.Interactions.Custom;
#else
namespace Avalonia.Xaml.Interactions.Custom;
#endif

/// <summary>
/// Binds the <see cref="InputElement.IsPointerOverProperty"/> to the <see cref="IsPointerOver"/> property.
/// </summary>
public partial class BindPointerOverBehavior : DisposingBehavior<Control>
{

    /// <summary>
    /// 
    /// </summary>
	[StyledProperty(DefaultBindingMode = PropertyBindingMode.TwoWay)]
	public partial bool IsPointerOver { get; set; }

    /// <summary>
    /// Called when the behavior is attached to the control.
    /// </summary>
    /// <returns>A disposable that removes the event handler.</returns>
        protected override IDisposable OnAttachedOverride()
	{
		if (AssociatedObject is null)
		{
			return DisposableAction.Empty;
		}

        var control = AssociatedObject;
#if UNO
        // WinUI has no IsPointerOver property: track the pointer enter/exit events instead.
        control.PointerEntered += AssociatedObjectOnPointerEntered;
        control.PointerExited += AssociatedObjectOnPointerExited;
        control.PointerCanceled += AssociatedObjectOnPointerExited;

        return DisposableAction.Create(() =>
        {
            control.PointerEntered -= AssociatedObjectOnPointerEntered;
            control.PointerExited -= AssociatedObjectOnPointerExited;
            control.PointerCanceled -= AssociatedObjectOnPointerExited;
            IsPointerOver = false;
        });

        void AssociatedObjectOnPointerEntered(object? sender, PointerEventArgs e) => IsPointerOver = true;

        void AssociatedObjectOnPointerExited(object? sender, PointerEventArgs e) => IsPointerOver = false;
#else
        control.PropertyChanged += AssociatedObjectOnPropertyChanged;

        return DisposableAction.Create(() =>
        {
            control.PropertyChanged -= AssociatedObjectOnPropertyChanged;
            IsPointerOver = false;
        });

        void AssociatedObjectOnPropertyChanged(object? sender, AvaloniaPropertyChangedEventArgs e)
        {
            if (e.Property == InputElement.IsPointerOverProperty)
            {
                IsPointerOver = e.NewValue is true;
            }
        }
#endif
	}
}
