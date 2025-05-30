using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.LogicalTree;
using Avalonia.Threading;

namespace Avalonia.Xaml.Interactions.Core;

/// <summary>
/// Asynchronously uploads a file to a specified URL and invokes a command when completed.
/// </summary>
public class UploadFileAction : InvokeCommandActionBase
{
    /// <summary>
    /// Identifies the <see cref="FilePath"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<string?> FilePathProperty =
        AvaloniaProperty.Register<UploadFileAction, string?>(nameof(FilePath));

    /// <summary>
    /// Identifies the <see cref="Url"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<string?> UrlProperty =
        AvaloniaProperty.Register<UploadFileAction, string?>(nameof(Url));

    /// <summary>
    /// Gets or sets the path of the file to upload. This is an avalonia property.
    /// </summary>
    public string? FilePath
    {
        get => GetValue(FilePathProperty);
        set => SetValue(FilePathProperty, value);
    }

    /// <summary>
    /// Gets or sets the destination URL. This is an avalonia property.
    /// </summary>
    public string? Url
    {
        get => GetValue(UrlProperty);
        set => SetValue(UrlProperty, value);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="UploadFileAction"/> class.
    /// </summary>
    public UploadFileAction()
    {
        PassEventArgsToCommand = true;
    }

    /// <inheritdoc />
    public override object Execute(object? sender, object? parameter)
    {
        if (sender is not Visual visual)
        {
            return false;
        }

        Dispatcher.UIThread.InvokeAsync(async () => await UploadAsync(visual));

        return true;
    }

    private async Task UploadAsync(Visual visual)
    {
        if (IsEnabled != true || Command is null || FilePath is null || Url is null)
        {
            return;
        }

        if (!File.Exists(FilePath))
        {
            return;
        }

        try
        {
            await using var stream = File.OpenRead(FilePath);
            using var content = new StreamContent(stream);
            using var client = new HttpClient();
            var response = await client.PostAsync(Url, content);

            var resolvedParameter = ResolveParameter(response);

            if (!Command.CanExecute(resolvedParameter))
            {
                return;
            }

            Command.Execute(resolvedParameter);
        }
        catch (Exception)
        {
            // ignored
        }
    }
}
