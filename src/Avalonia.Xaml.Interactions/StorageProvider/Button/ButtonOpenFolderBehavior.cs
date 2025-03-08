using Avalonia.Controls;
using Avalonia.Interactivity;

namespace Avalonia.Xaml.Interactions.Core;

/// <summary>
/// Open folder behavior for <see cref="Button"/>.
/// </summary>
public class ButtonOpenFolderBehavior : OpenFolderBehaviorBase
{
    /// <inheritdoc />
    protected override void OnAttachedToVisualTree()
    {
        base.OnAttachedToVisualTree();
        
        if (AssociatedObject is Button button)
        {
            button.Click += OnClick;
        }
    }

    /// <inheritdoc />
    protected override void OnDetachedFromVisualTree()
    {
        base.OnDetachedFromVisualTree();
        
        if (AssociatedObject is Button button)
        {
            button.Click += OnClick;
        }
    }

    // ReSharper disable once AsyncVoidMethod
    private async void OnClick(object? sender, RoutedEventArgs e)
    {
        await Execute(sender, e);
    }
}
