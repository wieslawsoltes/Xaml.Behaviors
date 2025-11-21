// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using System;
using Avalonia.Controls;
using Avalonia.Xaml.Interactivity;

namespace Avalonia.Xaml.Interactions.Custom;

/// <summary>
/// An action that launches a URI using the system's default handler.
/// </summary>
public class LaunchUriAction : StyledElementAction
{
    /// <summary>
    /// Identifies the <seealso cref="Uri"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<string?> UriProperty =
        AvaloniaProperty.Register<LaunchUriAction, string?>(nameof(Uri));

    /// <summary>
    /// Gets or sets the URI to launch. This is an avalonia property.
    /// </summary>
    public string? Uri
    {
        get => GetValue(UriProperty);
        set => SetValue(UriProperty, value);
    }

    /// <inheritdoc />
    public override object Execute(object? sender, object? parameter)
    {
        if (!IsEnabled)
        {
            return false;
        }

        var uriString = Uri;
        if (string.IsNullOrEmpty(uriString))
        {
            return false;
        }

        if (!System.Uri.TryCreate(uriString, UriKind.Absolute, out var uri))
        {
            return false;
        }

        var topLevel = TopLevel.GetTopLevel(sender as Visual);
        if (topLevel?.Launcher is { } launcher)
        {
            // Fire and forget
            _ = launcher.LaunchUriAsync(uri);
            return true;
        }

        return false;
    }
}
