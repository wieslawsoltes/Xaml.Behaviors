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
/// for offset changes only, so extent changes are observed through the size of the scrolled content.
/// </remarks>
public partial class AutoScrollToBottomBehavior
{
    private FrameworkElement? _scrollContent;
    private double _extentHeight;

    private void SubscribeScrollChanged(ScrollViewer scrollViewer)
    {
        _extentHeight = scrollViewer.ExtentHeight;
        scrollViewer.ViewChanged += OnViewChanged;
        scrollViewer.SizeChanged += OnScrollViewerSizeChanged;
        UpdateScrollContent(scrollViewer);
    }

    private void UnsubscribeScrollChanged(ScrollViewer scrollViewer)
    {
        scrollViewer.ViewChanged -= OnViewChanged;
        scrollViewer.SizeChanged -= OnScrollViewerSizeChanged;
        SetScrollContent(null);
    }

    private void OnViewChanged(object? sender, ScrollViewerViewChangedEventArgs e)
    {
        if (!e.IsIntermediate)
        {
            OnExtentMaybeChanged();
        }
    }

    private void OnScrollViewerSizeChanged(object sender, SizeChangedEventArgs e)
    {
        if (sender is ScrollViewer scrollViewer)
        {
            UpdateScrollContent(scrollViewer);
        }
    }

    private void OnScrollContentSizeChanged(object sender, SizeChangedEventArgs e) => OnExtentMaybeChanged();

    private void OnExtentMaybeChanged()
    {
        if (_scrollViewer is null)
        {
            return;
        }

        var extentHeight = _scrollViewer.ExtentHeight;
        var extentDelta = extentHeight - _extentHeight;
        _extentHeight = extentHeight;
        OnScrollChanged(extentDelta);
    }

    private void UpdateScrollContent(ScrollViewer scrollViewer) => SetScrollContent(scrollViewer.Content as FrameworkElement);

    private void SetScrollContent(FrameworkElement? content)
    {
        if (ReferenceEquals(_scrollContent, content))
        {
            return;
        }

        if (_scrollContent is not null)
        {
            _scrollContent.SizeChanged -= OnScrollContentSizeChanged;
        }

        _scrollContent = content;

        if (_scrollContent is not null)
        {
            _scrollContent.SizeChanged += OnScrollContentSizeChanged;
        }
    }

    private static bool IsScrolledToBottom(ScrollViewer scrollViewer)
        => scrollViewer.VerticalOffset >= scrollViewer.ExtentHeight - scrollViewer.ViewportHeight - 1.0;

    private static void ScrollToEnd(ScrollViewer scrollViewer)
        => scrollViewer.ChangeView(null, scrollViewer.ScrollableHeight, null, disableAnimation: true);
}
