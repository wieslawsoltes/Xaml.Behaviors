// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
#if UNO
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Xaml.Interactivity;
#else
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Xaml.Interactivity;
#endif

#if UNO
namespace Xaml.Interactions.Custom;
#else
namespace Avalonia.Xaml.Interactions.Custom;
#endif

/// <summary>
/// Executes actions when the search box text changes.
/// </summary>
public sealed partial class TreeViewFilterTextChangedTrigger : InteractiveTriggerBase
{

    /// <summary>
    /// Gets or sets the search box control.
    /// </summary>
    [StyledProperty(ResolveByName = true)]
    public partial TextBox? SearchBox { get; set; }

    /// <inheritdoc />
    protected override void OnAttachedToVisualTree()
    {
#if UNO
        // WinUI TextChanged is a CLR event (no routing).
        if (SearchBox is not null)
        {
            SearchBox.TextChanged += OnTextChanged;
        }
#else
        SearchBox?.AddHandler(TextBox.TextChangedEvent, OnTextChanged, RoutingStrategies);
#endif
    }

    /// <inheritdoc />
    protected override void OnDetachedFromVisualTree()
    {
#if UNO
        if (SearchBox is not null)
        {
            SearchBox.TextChanged -= OnTextChanged;
        }
#else
        SearchBox?.RemoveRoutedEventHandler(TextBox.TextChangedEvent, OnTextChanged);
#endif
    }

    private void OnTextChanged(object? sender, RoutedEventArgs e)
    {
        Execute(e);
    }
}
