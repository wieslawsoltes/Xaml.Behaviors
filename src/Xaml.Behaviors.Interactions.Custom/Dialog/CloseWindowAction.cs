// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using Avalonia.Controls;
using Avalonia.Xaml.Interactivity;

namespace Avalonia.Xaml.Interactions.Custom;

/// <summary>
/// Closes a <see cref="Window"/> when executed.
/// </summary>
public class CloseWindowAction : StyledElementAction
{
    /// <summary>
    /// Identifies the <seealso cref="Window"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<Window?> WindowProperty =
        AvaloniaProperty.Register<CloseWindowAction, Window?>(nameof(Window));

    /// <summary>
    /// Gets or sets the window instance to close. This is an avalonia property.
    /// </summary>
    [ResolveByName]
    public Window? Window
    {
        get => GetValue(WindowProperty);
        set => SetValue(WindowProperty, value);
    }

    /// <inheritdoc />
    public override object Execute(object? sender, object? parameter)
    {
        if (!IsEnabled)
        {
            return false;
        }

        var window = Window ?? sender as Window;
        if (window is null)
        {
            return false;
        }

        window.Close();
        return true;
    }
}
