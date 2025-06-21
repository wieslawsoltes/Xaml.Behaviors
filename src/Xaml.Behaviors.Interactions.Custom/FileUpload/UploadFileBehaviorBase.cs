// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Avalonia.Xaml.Interactivity;

namespace Avalonia.Xaml.Interactions.Custom;

/// <summary>
/// Base class for behaviors that upload a file to a URL.
/// </summary>
public abstract class UploadFileBehaviorBase : InvokeCommandBehaviorBase
{
    /// <summary>
    /// Identifies the <see cref="FilePath"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<string?> FilePathProperty =
        AvaloniaProperty.Register<UploadFileBehaviorBase, string?>(nameof(FilePath));

    /// <summary>
    /// Identifies the <see cref="Url"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<string?> UrlProperty =
        AvaloniaProperty.Register<UploadFileBehaviorBase, string?>(nameof(Url));

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
    /// Initializes a new instance of the <see cref="UploadFileBehaviorBase"/> class.
    /// </summary>
    protected UploadFileBehaviorBase()
    {
        PassEventArgsToCommand = true;
    }

    /// <summary>
    /// Executes the upload using the provided sender and command parameter.
    /// </summary>
    /// <param name="sender">The control that triggered the behavior.</param>
    /// <param name="parameter">Optional parameter.</param>
    protected async Task Execute(object? sender, object? parameter)
    {
        if (sender is not Visual)
        {
            return;
        }

        await UploadAsync();
    }

    private async Task UploadAsync()
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
#if NET6_0_OR_GREATER
            await using var stream = File.OpenRead(FilePath);
#else
            using var stream = File.OpenRead(FilePath);
#endif
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
