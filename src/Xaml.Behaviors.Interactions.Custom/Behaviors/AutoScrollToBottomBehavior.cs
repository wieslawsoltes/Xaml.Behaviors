using System;
using System.Collections.Specialized;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Threading;
using Avalonia.VisualTree;
using Avalonia.Xaml.Interactivity;

namespace Avalonia.Xaml.Interactions.Custom;

/// <summary>
/// A behavior that automatically scrolls to the bottom of a ScrollViewer or ItemsControl when new items are added.
/// </summary>
public class AutoScrollToBottomBehavior : StyledElementBehavior<Control>
{
    private ScrollViewer? _scrollViewer;
    private INotifyCollectionChanged? _items;
    private bool _autoScroll = true;

    /// <summary>
    /// Identifies the <seealso cref="ItemsSource"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<object?> ItemsSourceProperty =
        AvaloniaProperty.Register<AutoScrollToBottomBehavior, object?>(nameof(ItemsSource));

    /// <summary>
    /// Gets or sets the items source to monitor for changes.
    /// </summary>
    public object? ItemsSource
    {
        get => GetValue(ItemsSourceProperty);
        set => SetValue(ItemsSourceProperty, value);
    }

    /// <inheritdoc />
    protected override void OnAttached()
    {
        base.OnAttached();
        
        if (AssociatedObject is ScrollViewer scrollViewer)
        {
            _scrollViewer = scrollViewer;
            _scrollViewer.ScrollChanged += OnScrollChanged;
        }
        else if (AssociatedObject is ItemsControl itemsControl)
        {
             // Try to find ScrollViewer immediately
             _scrollViewer = itemsControl.FindDescendantOfType<ScrollViewer>();
             if (_scrollViewer is not null)
             {
                 _scrollViewer.ScrollChanged += OnScrollChanged;
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
                             _scrollViewer.ScrollChanged += OnScrollChanged;
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
            _scrollViewer.ScrollChanged -= OnScrollChanged;
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
        else if (itemsSource is null && AssociatedObject is ItemsControl itemsControl && itemsControl.Items is INotifyCollectionChanged itemsCollection)
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

    private void OnScrollChanged(object? sender, ScrollChangedEventArgs e)
    {
        if (_scrollViewer is null)
        {
            return;
        }

        if (e.ExtentDelta.Y == 0)
        {
            // User scroll
            if (_scrollViewer.Offset.Y >= _scrollViewer.Extent.Height - _scrollViewer.Viewport.Height - 1.0)
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
                 _scrollViewer.ScrollChanged += OnScrollChanged;
             }
        }

        if (_scrollViewer is not null)
        {
            Dispatcher.UIThread.Post(() => 
            {
                _scrollViewer.ScrollToEnd();
            });
        }
    }
}
