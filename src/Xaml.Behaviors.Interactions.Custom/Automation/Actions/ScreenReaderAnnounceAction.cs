// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using System.Reflection;
using Avalonia.Controls;
using Avalonia.Xaml.Interactivity;

namespace Avalonia.Xaml.Interactions.Custom;

/// <summary>
/// An action that requests a screen reader announcement.
/// </summary>
public class ScreenReaderAnnounceAction : StyledElementAction
{
    /// <summary>
    /// Identifies the <seealso cref="Message"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<string?> MessageProperty =
        AvaloniaProperty.Register<ScreenReaderAnnounceAction, string?>(nameof(Message));

    /// <summary>
    /// Gets or sets the message to announce.
    /// </summary>
    public string? Message
    {
        get => GetValue(MessageProperty);
        set => SetValue(MessageProperty, value);
    }

    /// <inheritdoc />
    public override object? Execute(object? sender, object? parameter)
    {
        if (sender is Control control && !string.IsNullOrEmpty(Message))
        {
            var topLevel = TopLevel.GetTopLevel(control);
            if (topLevel != null)
            {
                var method = typeof(TopLevel).GetMethod("RequestScreenReaderAnnouncement");
                if (method != null)
                {
                    method.Invoke(topLevel, new object?[] { Message });
                    return true;
                }
            }
        }
        return false;
    }
}
