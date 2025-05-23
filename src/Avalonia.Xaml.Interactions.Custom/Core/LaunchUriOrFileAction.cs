using System;
using System.Diagnostics;
using Avalonia.Xaml.Interactivity;

namespace Avalonia.Xaml.Interactions.Custom;

/// <summary>
/// An action that will launch a process to open a file or URI. For files, this action will launch the
/// default program for the given file extension. A URI will open in a web browser.
/// </summary>
public class LaunchUriOrFileAction : StyledElementAction
{
    /// <summary>
    /// Identifies the <see cref="Path"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<string?> PathProperty =
        AvaloniaProperty.Register<LaunchUriOrFileAction, string?>(nameof(Path));

    /// <summary>
    /// Gets or sets the file or URI to open. This is an avalonia property.
    /// </summary>
    public string? Path
    {
        get => GetValue(PathProperty);
        set => SetValue(PathProperty, value);
    }

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
