// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
#if UNO
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
#else
using Avalonia.Controls;
using Avalonia.Interactivity;
#endif

#if UNO
namespace Xaml.Interactions.Core;
#else
namespace Avalonia.Xaml.Interactions.Core;
#endif

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
