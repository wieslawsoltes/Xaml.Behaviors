using System;
using System.IO;
using Avalonia.Xaml.Interactivity;

namespace Avalonia.Xaml.Interactions.FileSystem;

/// <summary>
/// An action that writes text to a file.
/// </summary>
public class WriteTextToFileAction : StyledElementAction
{
    /// <summary>
    /// Identifies the <seealso cref="Path"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<string?> PathProperty =
        AvaloniaProperty.Register<WriteTextToFileAction, string?>(nameof(Path));

    /// <summary>
    /// Identifies the <seealso cref="Text"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<string?> TextProperty =
        AvaloniaProperty.Register<WriteTextToFileAction, string?>(nameof(Text));

    /// <summary>
    /// Identifies the <seealso cref="Append"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<bool> AppendProperty =
        AvaloniaProperty.Register<WriteTextToFileAction, bool>(nameof(Append));

    /// <summary>
    /// Gets or sets the path of the file.
    /// </summary>
    public string? Path
    {
        get => GetValue(PathProperty);
        set => SetValue(PathProperty, value);
    }

    /// <summary>
    /// Gets or sets the text to write.
    /// </summary>
    public string? Text
    {
        get => GetValue(TextProperty);
        set => SetValue(TextProperty, value);
    }

    /// <summary>
    /// Gets or sets a value indicating whether to append text to the file.
    /// </summary>
    public bool Append
    {
        get => GetValue(AppendProperty);
        set => SetValue(AppendProperty, value);
    }

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
