// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using System;
#if UNO
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Xaml.Interactivity;
#else
using Avalonia.Controls;
using Avalonia.Xaml.Interactivity;
#endif

#if UNO
namespace Xaml.Interactions.Custom;
#else
namespace Avalonia.Xaml.Interactions.Custom;
#endif

/// <summary>
/// An action that launches a URI using the system's default handler.
/// </summary>
public partial class LaunchUriAction : StyledElementAction
{

    /// <summary>
    /// Gets or sets the URI to launch. This is an avalonia property.
    /// </summary>
    [StyledProperty]
    public partial string? Uri { get; set; }

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

#if UNO
        // Fire and forget
        _ = Windows.System.Launcher.LaunchUriAsync(uri);
        return true;
#else
        var topLevel = TopLevel.GetTopLevel(sender as Visual);
        if (topLevel?.Launcher is { } launcher)
        {
            // Fire and forget
            _ = launcher.LaunchUriAsync(uri);
            return true;
        }

        return false;
#endif
    }
}
