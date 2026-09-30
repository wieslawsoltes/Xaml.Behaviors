using System;
using System.Collections.Specialized;
#if UNO
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Dispatching;
using Xaml.Interactivity;
#else
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Threading;
using Avalonia.VisualTree;
using Avalonia.Xaml.Interactivity;
#endif

#if UNO
namespace Xaml.Interactions.Custom;
#else
namespace Avalonia.Xaml.Interactions.Custom;
#endif

/// <summary>
/// A behavior that automatically scrolls to the bottom of a ScrollViewer or ItemsControl when new items are added.
/// </summary>
public partial class AutoScrollToBottomBehavior : StyledElementBehavior<Control>
{
    private ScrollViewer? _scrollViewer;
    private INotifyCollectionChanged? _items;
    private bool _autoScroll = true;

    /// <summary>
    /// Gets or sets the items source to monitor for changes.
    /// </summary>
    [StyledProperty]
    public partial object? ItemsSource { get; set; }

    /// <inheritdoc />
    protected override void OnAttached()
    {
        base.OnAttached();
        
        if (AssociatedObject is ScrollViewer scrollViewer)
        {
            _scrollViewer = scrollViewer;
            SubscribeScrollChanged(_scrollViewer);
        }
        else if (AssociatedObject is ItemsControl itemsControl)
        {
             // Try to find ScrollViewer immediately
             _scrollViewer = itemsControl.FindDescendantOfType<ScrollViewer>();
             if (_scrollViewer is not null)
             {
                 SubscribeScrollChanged(_scrollViewer);
             }
             else
             {
                 // Try later
                 Dispatcher.UIThread.Post(() => 
                 {
                     if (AssociatedObject == itemsControl)
                     {
                         _scrollViewer = itemsControl.FindDescendantOfType<ScrollViewer>();
                         if (_scrollViewer is not null)
                         {
                             SubscribeScrollChanged(_scrollViewer);
                         }
                     }
                 });
             }
        }
        
        UpdateItemsSource(ItemsSource);
    }

    /// <inheritdoc />
    protected override void OnDetaching()
    {
        base.OnDetaching();
        
        if (_scrollViewer is not null)
        {
            UnsubscribeScrollChanged(_scrollViewer);
            _scrollViewer = null;
        }
        
        if (_items is not null)
        {
            _items.CollectionChanged -= OnCollectionChanged;
            _items = null;
        }
    }

    /// <inheritdoc />
    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        
        if (change.Property == ItemsSourceProperty)
        {
            UpdateItemsSource(change.GetNewValue<object?>());
        }
    }

    private void UpdateItemsSource(object? itemsSource)
    {
        if (_items is not null)
        {
            _items.CollectionChanged -= OnCollectionChanged;
            _items = null;
        }

        if (itemsSource is INotifyCollectionChanged items)
        {
            _items = items;
            _items.CollectionChanged += OnCollectionChanged;
        }
#if UNO
        // WinUI item collections are vectors; the bound items source carries the collection notifications.
        else if (itemsSource is null && AssociatedObject is ItemsControl itemsControl && itemsControl.ItemsSource is INotifyCollectionChanged itemsCollection)
#else
        else if (itemsSource is null && AssociatedObject is ItemsControl itemsControl && itemsControl.Items is INotifyCollectionChanged itemsCollection)
#endif
        {
            // Fallback to ItemsControl.Items if ItemsSource is not set
            _items = itemsCollection;
            _items.CollectionChanged += OnCollectionChanged;
        }
    }

    private void OnCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        if (e.Action == NotifyCollectionChangedAction.Add)
        {
            if (_autoScroll)
            {
                ScrollToBottom();
            }
        }
    }

#if !UNO
    private void SubscribeScrollChanged(ScrollViewer scrollViewer) => scrollViewer.ScrollChanged += OnScrollChanged;

    private void UnsubscribeScrollChanged(ScrollViewer scrollViewer) => scrollViewer.ScrollChanged -= OnScrollChanged;

    private void OnScrollChanged(object? sender, ScrollChangedEventArgs e) => OnScrollChanged(e.ExtentDelta.Y);

    private static bool IsScrolledToBottom(ScrollViewer scrollViewer)
        => scrollViewer.Offset.Y >= scrollViewer.Extent.Height - scrollViewer.Viewport.Height - 1.0;

    private static void ScrollToEnd(ScrollViewer scrollViewer) => scrollViewer.ScrollToEnd();
#endif

    private void OnScrollChanged(double extentDeltaY)
    {
        if (_scrollViewer is null)
        {
            return;
        }

        if (extentDeltaY == 0)
        {
            // User scroll
            if (IsScrolledToBottom(_scrollViewer))
            {
                _autoScroll = true;
            }
            else
            {
                _autoScroll = false;
            }
        }
        else
        {
            // Content changed
            if (_autoScroll)
            {
                ScrollToBottom();
            }
        }
    }

    private void ScrollToBottom()
    {
        if (_scrollViewer is null && AssociatedObject is ItemsControl itemsControl)
        {
             _scrollViewer = itemsControl.FindDescendantOfType<ScrollViewer>();
             if (_scrollViewer is not null)
             {
                 SubscribeScrollChanged(_scrollViewer);
             }
        }

        if (_scrollViewer is not null)
        {
            Dispatcher.UIThread.Post(() => 
            {
                if(_scrollViewer is not null)
                {
                    ScrollToEnd(_scrollViewer);
                }
            });
        }
    }
}
