using System;
using System.IO;
using Avalonia.Xaml.Interactivity;

namespace Avalonia.Xaml.Interactions.FileSystem;

/// <summary>
/// An action that creates a directory at the specified path.
/// </summary>
public class CreateDirectoryAction : StyledElementAction
{
    /// <summary>
    /// Identifies the <seealso cref="Path"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<string?> PathProperty =
        AvaloniaProperty.Register<CreateDirectoryAction, string?>(nameof(Path));

    /// <summary>
    /// Gets or sets the path of the directory to create.
    /// </summary>
    public string? Path
    {
        get => GetValue(PathProperty);
        set => SetValue(PathProperty, value);
    }

    /// <inheritdoc />
    public override object? Execute(object? sender, object? parameter)
    {
        var path = Path;
        if (!string.IsNullOrEmpty(path))
        {
            try
            {
                Directory.CreateDirectory(path);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        return false;
    }
}
