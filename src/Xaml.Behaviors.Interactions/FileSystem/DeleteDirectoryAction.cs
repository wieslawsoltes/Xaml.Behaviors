using System;
using System.IO;
using Avalonia.Xaml.Interactivity;

namespace Avalonia.Xaml.Interactions.FileSystem;

/// <summary>
/// An action that deletes a directory at the specified path.
/// </summary>
public class DeleteDirectoryAction : StyledElementAction
{
    /// <summary>
    /// Identifies the <seealso cref="Path"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<string?> PathProperty =
        AvaloniaProperty.Register<DeleteDirectoryAction, string?>(nameof(Path));

    /// <summary>
    /// Identifies the <seealso cref="Recursive"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<bool> RecursiveProperty =
        AvaloniaProperty.Register<DeleteDirectoryAction, bool>(nameof(Recursive), true);

    /// <summary>
    /// Gets or sets the path of the directory to delete.
    /// </summary>
    public string? Path
    {
        get => GetValue(PathProperty);
        set => SetValue(PathProperty, value);
    }

    /// <summary>
    /// Gets or sets a value indicating whether to delete subdirectories and files.
    /// </summary>
    public bool Recursive
    {
        get => GetValue(RecursiveProperty);
        set => SetValue(RecursiveProperty, value);
    }

    /// <inheritdoc />
    public override object? Execute(object? sender, object? parameter)
    {
        var path = Path;
        if (!string.IsNullOrEmpty(path) && Directory.Exists(path))
        {
            try
            {
                Directory.Delete(path, Recursive);
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
