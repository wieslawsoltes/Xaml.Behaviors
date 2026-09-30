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
/// An action that deletes a file at the specified path.
/// </summary>
public partial class DeleteFileAction : StyledElementAction
{

    /// <summary>
    /// Gets or sets the path of the file to delete.
    /// </summary>
    [StyledProperty]
    public partial string? Path { get; set; }

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
