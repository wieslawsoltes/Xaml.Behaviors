using System;
using System.IO;
using Avalonia.Xaml.Interactivity;

namespace Avalonia.Xaml.Interactions.FileSystem;

/// <summary>
/// An action that deletes a file at the specified path.
/// </summary>
public class DeleteFileAction : StyledElementAction
{
    /// <summary>
    /// Identifies the <seealso cref="Path"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<string?> PathProperty =
        AvaloniaProperty.Register<DeleteFileAction, string?>(nameof(Path));

    /// <summary>
    /// Gets or sets the path of the file to delete.
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
        if (!string.IsNullOrEmpty(path) && File.Exists(path))
        {
            try
            {
                File.Delete(path);
                return true;
            }
            catch (Exception)
            {
                // TODO: Handle exception or log it?
                return false;
            }
        }
        return false;
    }
}
