using Avalonia.Controls;
using Avalonia.Interactivity;

namespace Avalonia.Xaml.Interactions.Core;

/// <summary>
/// Open folder picker behavior for <see cref="MenuItem"/>.
/// </summary>
public class MenuItemOpenFolderPickerBehavior : OpenFolderPickerBehaviorBase
{
    /// <inheritdoc />
    protected override void OnAttachedToVisualTree()
    {
        base.OnAttachedToVisualTree();
        
        if (AssociatedObject is MenuItem menuItem)
        {
            menuItem.Click += OnClick;
        }
    }

    /// <inheritdoc />
    protected override void OnDetachedFromVisualTree()
    {
        base.OnDetachedFromVisualTree();
        
        if (AssociatedObject is MenuItem menuItem)
        {
            menuItem.Click -= OnClick;
        }
    }

    // ReSharper disable once AsyncVoidMethod
    private async void OnClick(object? sender, RoutedEventArgs e)
    {
        await Execute(sender, e);
    }
}
