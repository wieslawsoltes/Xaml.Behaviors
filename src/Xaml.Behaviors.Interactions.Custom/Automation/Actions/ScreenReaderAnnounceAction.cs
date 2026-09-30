// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using System.Reflection;
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
/// An action that requests a screen reader announcement.
/// </summary>
public partial class ScreenReaderAnnounceAction : StyledElementAction
{

    /// <summary>
    /// Gets or sets the message to announce.
    /// </summary>
    [StyledProperty]
    public partial string? Message { get; set; }

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
