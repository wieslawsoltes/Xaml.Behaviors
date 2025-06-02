using System;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Media.Imaging;
using Avalonia.Threading;
using Avalonia.Xaml.Interactivity;

namespace Avalonia.Xaml.Interactions.Custom;

/// <summary>
/// Loads an image asynchronously when attached or when <see cref="Source"/> changes.
/// </summary>
public class LoadImageBehavior : StyledElementBehavior<Image>
{
    /// <summary>
    /// Identifies the <seealso cref="Source"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<Uri?> SourceProperty =
        AvaloniaProperty.Register<LoadImageBehavior, Uri?>(nameof(Source));

    /// <summary>
    /// Identifies the <seealso cref="AutoLoad"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<bool> AutoLoadProperty =
        AvaloniaProperty.Register<LoadImageBehavior, bool>(nameof(AutoLoad), true);

    /// <summary>
    /// Gets or sets the image source URI. This is an avalonia property.
    /// </summary>
    public Uri? Source
    {
        get => GetValue(SourceProperty);
        set => SetValue(SourceProperty, value);
    }

    /// <summary>
    /// Gets or sets a value indicating whether the image is loaded automatically when <see cref="Source"/> changes. This is an avalonia property.
    /// </summary>
    public bool AutoLoad
    {
        get => GetValue(AutoLoadProperty);
        set => SetValue(AutoLoadProperty, value);
    }

    /// <inheritdoc />
    protected override void OnAttached()
    {
        base.OnAttached();

        if (AutoLoad)
        {
            _ = LoadAsync();
        }
    }

    /// <inheritdoc />
    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);

        if (change.Property == SourceProperty && AutoLoad)
        {
            _ = LoadAsync();
        }
    }

    /// <summary>
    /// Loads the image asynchronously.
    /// </summary>
    public async Task LoadAsync()
    {
        if (!IsEnabled || AssociatedObject is null || Source is null)
        {
            return;
        }

        try
        {
            var bitmap = await Task.Run(() => new Bitmap(Source));
            await Dispatcher.UIThread.InvokeAsync(() =>
            {
                if (AssociatedObject is not null)
                {
                    AssociatedObject.Source = bitmap;
                }
            });
        }
        catch (Exception)
        {
            // ignored
        }
    }
}
