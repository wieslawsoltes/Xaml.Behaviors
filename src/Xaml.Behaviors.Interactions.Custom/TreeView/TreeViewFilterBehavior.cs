// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
#if UNO
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Xaml.Interactivity;
#else
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Xaml.Interactivity;
#endif

#if UNO
namespace Xaml.Interactions.Custom;
#else
namespace Avalonia.Xaml.Interactions.Custom;
#endif

/// <summary>
/// Filters <see cref="TreeView"/> items based on the text of a search box.
/// </summary>
public sealed partial class TreeViewFilterBehavior : StyledElementBehavior<TreeView>
{

    /// <summary>
    /// Gets or sets the search box control.
    /// </summary>
    [StyledProperty(ResolveByName = true)]
    public partial TextBox? SearchBox { get; set; }

    /// <summary>
    /// Gets or sets the control displayed when no matches are found.
    /// </summary>
    [StyledProperty(ResolveByName = true)]
    public partial Control? NoMatchesControl { get; set; }

    /// <inheritdoc />
    protected override void OnAttachedToVisualTree()
    {
        if (SearchBox is not null)
        {
#if UNO
            // WinUI raises TextChanged (a CLR event) after every edit, including text input.
            SearchBox.TextChanged += SearchBox_TextChanged;
#else
            SearchBox.AddHandler(InputElement.TextInputEvent, SearchBox_TextChanged, RoutingStrategies.Bubble);
            SearchBox.AddHandler(TextBox.TextChangedEvent, SearchBox_TextChanged, RoutingStrategies.Bubble);
#endif
        }
    }

    /// <inheritdoc />
    protected override void OnDetachedFromVisualTree()
    {
        if (SearchBox is not null)
        {
#if UNO
            SearchBox.TextChanged -= SearchBox_TextChanged;
#else
            SearchBox.RemoveRoutedEventHandler(InputElement.TextInputEvent, SearchBox_TextChanged);
            SearchBox.RemoveRoutedEventHandler(TextBox.TextChangedEvent, SearchBox_TextChanged);
#endif
        }
    }

    private void SearchBox_TextChanged(object? sender, RoutedEventArgs e)
    {
        if (AssociatedObject is null)
        {
            return;
        }

        var query = SearchBox?.Text ?? string.Empty;
        var count = TreeViewFilterHelper.Filter(AssociatedObject, query);

        if (NoMatchesControl is not null)
        {
            NoMatchesControl.IsVisible = count == 0;
        }
    }
}
