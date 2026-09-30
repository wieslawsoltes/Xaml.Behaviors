// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using System;
using System.Diagnostics;
#if UNO
using Xaml.Interactivity;
#else
using Avalonia.Xaml.Interactivity;
#endif

#if UNO
namespace Xaml.Interactions.Custom;
#else
namespace Avalonia.Xaml.Interactions.Custom;
#endif

/// <summary>
/// An action that will launch a process to open a file or URI. For files, this action will launch the
/// default program for the given file extension. A URI will open in a web browser.
/// </summary>
public partial class LaunchUriOrFileAction : StyledElementAction
{

    /// <summary>
    /// Gets or sets the file or URI to open. This is an avalonia property.
    /// </summary>
    [StyledProperty]
    public partial string? Path { get; set; }

    /// <inheritdoc />
    public override object Execute(object? sender, object? parameter)
    {
        if (IsEnabled != true)
        {
            return false;
        }

        var target = GetValue(PathProperty);
        if (string.IsNullOrWhiteSpace(target))
        {
            return false;
        }

        try
        {
            var processStartInfo = new ProcessStartInfo(target)
            {
                UseShellExecute = true
            };
            Process.Start(processStartInfo);
        }
        catch (Exception)
        {
            return false;
        }

        return true;
    }
}
