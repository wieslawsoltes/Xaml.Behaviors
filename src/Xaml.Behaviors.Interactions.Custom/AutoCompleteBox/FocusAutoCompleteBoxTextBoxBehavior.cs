// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using System;
using System.Linq;
#if UNO
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Dispatching;
using Xaml.Interactivity;
using AutoCompleteBox = Microsoft.UI.Xaml.Controls.AutoSuggestBox;
#else
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Threading;
using Avalonia.VisualTree;
using Avalonia.Xaml.Interactivity;
#endif

#if UNO
namespace Xaml.Interactions.Custom;
#else
namespace Avalonia.Xaml.Interactions.Custom;
#endif

/// <summary>
/// Moves the focus to the text box of an <see cref="AutoCompleteBox"/> when the box receives focus.
/// </summary>
public class FocusAutoCompleteBoxTextBoxBehavior : AttachedToVisualTreeBehavior<AutoCompleteBox>
{
    /// <inheritdoc />
    protected override IDisposable OnAttachedToVisualTreeOverride()
    {
        if (AssociatedObject is null)
        {
            return DisposableAction.Empty;
        }

        AssociatedObject.GotFocus += AssociatedObjectOnGotFocus;

        return DisposableAction.Create(() => AssociatedObject.GotFocus -= AssociatedObjectOnGotFocus);
    }

    private void AssociatedObjectOnGotFocus(object? sender, FocusChangedEventArgs e)
    {
        var textBox = AssociatedObject?.GetVisualDescendants().OfType<TextBox>().FirstOrDefault();
        Dispatcher.UIThread.Post(() => textBox?.Focus());
    }
}
