// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using System;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Interactivity;
using Avalonia.LogicalTree;
using Avalonia.Xaml.Interactivity;

namespace Avalonia.Xaml.Interactions.Custom;

/// <summary>
/// Hides the popup when the button is clicked.
/// </summary>
public class ButtonHidePopupOnClickBehavior : AttachedToVisualTreeBehavior<Button>
{
    /// <inheritdoc />
    protected override IDisposable OnAttachedToVisualTreeOverride()
    {
        var button = AssociatedObject;

        if (button is null)
        {
            return DisposableAction.Empty;
        }

        var popup = button.FindLogicalAncestorOfType<Popup>();
        if (popup is null)
        {
            return DisposableAction.Empty;
        }

        button.Click += AssociatedObjectOnClick;

        return DisposableAction.Create(() =>
        {
            button.Click -= AssociatedObjectOnClick;
        });

        void AssociatedObjectOnClick(object? sender, RoutedEventArgs e)
        {
            if (button.Command != null && button.IsEnabled)
            {
                button.Command.Execute(button.CommandParameter);
            }

            popup.Close();
        }
    }
}
