using System;
using System.IO;
#if UNO
using Xaml.Interactivity;
#else
using Avalonia.Xaml.Interactivity;
#endif

#if UNO
namespace Xaml.Interactions.FileSystem;
#else
namespace Avalonia.Xaml.Interactions.FileSystem;
#endif

/// <summary>
/// An action that deletes a directory at the specified path.
/// </summary>
public partial class DeleteDirectoryAction : StyledElementAction
{

    /// <summary>
    /// Gets or sets the path of the directory to delete.
    /// </summary>
    [StyledProperty]
    public partial string? Path { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether to delete subdirectories and files.
    /// </summary>
    [StyledProperty(DefaultValue = true)]
    public partial bool Recursive { get; set; }

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
