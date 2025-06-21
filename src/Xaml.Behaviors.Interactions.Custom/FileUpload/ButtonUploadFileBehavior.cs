// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using Avalonia.Controls;
using Avalonia.Interactivity;

namespace Avalonia.Xaml.Interactions.Custom;

/// <summary>
/// Upload file behavior for <see cref="Button"/>.
/// </summary>
public class ButtonUploadFileBehavior : UploadFileBehaviorBase
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
            button.Click -= OnClick;
        }
    }

    // ReSharper disable once AsyncVoidMethod
    private async void OnClick(object? sender, RoutedEventArgs e)
    {
        await Execute(sender, e);
    }
}
