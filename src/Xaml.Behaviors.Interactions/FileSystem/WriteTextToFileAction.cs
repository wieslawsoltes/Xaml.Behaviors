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
/// An action that writes text to a file.
/// </summary>
public partial class WriteTextToFileAction : StyledElementAction
{

    /// <summary>
    /// Gets or sets the path of the file.
    /// </summary>
    [StyledProperty]
    public partial string? Path { get; set; }

    /// <summary>
    /// Gets or sets the text to write.
    /// </summary>
    [StyledProperty]
    public partial string? Text { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether to append text to the file.
    /// </summary>
    [StyledProperty]
    public partial bool Append { get; set; }

    /// <inheritdoc />
    public override object? Execute(object? sender, object? parameter)
    {
        var path = Path;
        var text = Text;
        if (!string.IsNullOrEmpty(path) && text != null)
        {
            try
            {
                if (Append)
                {
                    File.AppendAllText(path, text);
                }
                else
                {
                    File.WriteAllText(path, text);
                }
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
