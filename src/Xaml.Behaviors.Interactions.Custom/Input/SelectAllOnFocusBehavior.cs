// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
#if UNO
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Xaml.Interactivity;
#else
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Xaml.Interactivity;
#endif

#if UNO
namespace Xaml.Interactions.Custom;
#else
namespace Avalonia.Xaml.Interactions.Custom;
#endif

/// <summary>
/// A behavior that automatically selects all text in a TextBox when it receives focus.
/// </summary>
public class SelectAllOnFocusBehavior : Behavior<Control>
{
    /// <inheritdoc />
    protected override void OnAttachedToVisualTree()
    {
        base.OnAttachedToVisualTree();
        if (AssociatedObject is not null)
        {
            AssociatedObject.GotFocus += AssociatedObject_GotFocus;
        }
    }

    /// <inheritdoc />
    protected override void OnDetachedFromVisualTree()
    {
        base.OnDetachedFromVisualTree();
        if (AssociatedObject is not null)
        {
            AssociatedObject.GotFocus -= AssociatedObject_GotFocus;
        }
    }

    private void AssociatedObject_GotFocus(object? sender, FocusChangedEventArgs e)
    {
        if (AssociatedObject is TextBox textBox)
        {
            textBox.SelectAll();
        }
#if !UNO
        else if (AssociatedObject is SelectableTextBlock selectableTextBlock)
        {
            // SelectableTextBlock does not expose SelectAll public method easily or it might be different.
            // For now we support TextBox.
        }
#endif
    }
}
