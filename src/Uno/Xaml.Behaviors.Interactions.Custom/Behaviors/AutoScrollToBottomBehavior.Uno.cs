// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace Xaml.Interactions.Custom;

/// <content>
/// Uno Platform scroll notifications.
/// </content>
/// <remarks>
/// Avalonia raises <c>ScrollChanged</c> with the extent delta. WinUI raises <see cref="ScrollViewer.ViewChanged"/>
/// for offset changes only, so extent changes are observed through <see cref="ScrollViewer.ExtentHeightProperty"/>.
/// </remarks>
public partial class AutoScrollToBottomBehavior
{
    private long _extentHeightToken;
    private double _extentHeight;

    /// <inheritdoc />
    /// <remarks>
    /// WinUI attaches the behaviors when XAML sets them, before the view assigns the items: an Avalonia
    /// <c>ItemsControl.Items</c> follows a later <c>ItemsSource</c>, a WinUI item collection does not. Without an
    /// explicit <see cref="ItemsSource"/>, the items source of the control is observed when the control is loaded.
    /// </remarks>
    protected override void OnAttachedToVisualTree()
    {
        base.OnAttachedToVisualTree();

        if (_items is null)
        {
            UpdateItemsSource(ItemsSource);
        }
    }

    private void SubscribeScrollChanged(ScrollViewer scrollViewer)
    {
        _extentHeight = scrollViewer.ExtentHeight;
        scrollViewer.ViewChanged += OnViewChanged;
        _extentHeightToken = scrollViewer.RegisterPropertyChangedCallback(ScrollViewer.ExtentHeightProperty, OnExtentHeightChanged);
    }

    private void UnsubscribeScrollChanged(ScrollViewer scrollViewer)
    {
        scrollViewer.ViewChanged -= OnViewChanged;
        scrollViewer.UnregisterPropertyChangedCallback(ScrollViewer.ExtentHeightProperty, _extentHeightToken);
    }

    private void OnViewChanged(object? sender, ScrollViewerViewChangedEventArgs e)
    {
        if (!e.IsIntermediate)
        {
            OnScrollChanged(extentDeltaY: 0);
        }
    }

    private void OnExtentHeightChanged(DependencyObject sender, DependencyProperty property)
    {
        if (sender is not ScrollViewer scrollViewer)
        {
            return;
        }

        var extentHeight = scrollViewer.ExtentHeight;
        var extentDelta = extentHeight - _extentHeight;
        _extentHeight = extentHeight;
        OnScrollChanged(extentDelta);
    }

    private static bool IsScrolledToBottom(ScrollViewer scrollViewer)
        => scrollViewer.VerticalOffset >= scrollViewer.ExtentHeight - scrollViewer.ViewportHeight - 1.0;

    private static void ScrollToEnd(ScrollViewer scrollViewer)
        => scrollViewer.ChangeView(null, scrollViewer.ScrollableHeight, null, disableAnimation: true);
}
