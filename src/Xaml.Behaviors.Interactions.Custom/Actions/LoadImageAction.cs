using System;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Media.Imaging;
using Avalonia.VisualTree;
using Avalonia.Threading;

namespace Avalonia.Xaml.Interactions.Custom;

/// <summary>
/// Loads an image asynchronously and assigns it to a target <see cref="Image"/>.
/// </summary>
public class LoadImageAction : StyledElementAction
{
    /// <summary>
    /// Identifies the <seealso cref="Source"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<Uri?> SourceProperty =
        AvaloniaProperty.Register<LoadImageAction, Uri?>(nameof(Source));

    /// <summary>
    /// Identifies the <seealso cref="Target"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<Image?> TargetProperty =
        AvaloniaProperty.Register<LoadImageAction, Image?>(nameof(Target));

    /// <summary>
    /// Gets or sets the image source URI. This is an avalonia property.
    /// </summary>
    public Uri? Source
    {
        get => GetValue(SourceProperty);
        set => SetValue(SourceProperty, value);
    }

    /// <summary>
    /// Gets or sets the target image. This is an avalonia property.
    /// </summary>
    [ResolveByName]
    public Image? Target
    {
        get => GetValue(TargetProperty);
        set => SetValue(TargetProperty, value);
    }

    /// <inheritdoc />
    public override object Execute(object? sender, object? parameter)
    {
        if (sender is not Visual visual)
        {
            return false;
        }

        var target = Target ?? visual as Image;
        if (target is null)
        {
            return false;
        }

        Dispatcher.UIThread.InvokeAsync(async () => await LoadAsync(target));
        return true;
    }

    private async Task LoadAsync(Image target)
    {
        if (!IsEnabled || Source is null)
        {
            return;
        }

        try
        {
            var bitmap = await Task.Run(() => new Bitmap(Source));
            target.Source = bitmap;
        }
        catch (Exception)
        {
            // ignored
        }
    }
}
