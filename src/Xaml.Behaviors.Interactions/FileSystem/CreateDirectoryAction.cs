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
/// An action that creates a directory at the specified path.
/// </summary>
public partial class CreateDirectoryAction : StyledElementAction
{

    /// <summary>
    /// Gets or sets the path of the directory to create.
    /// </summary>
    [StyledProperty]
    public partial string? Path { get; set; }

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
