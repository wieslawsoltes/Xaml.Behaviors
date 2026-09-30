// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
#if UNO
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using TabControl = Microsoft.UI.Xaml.Controls.TabView;
#else
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Interactivity;
#endif

#if UNO
namespace Xaml.Interactions.Custom;
#else
namespace Avalonia.Xaml.Interactions.Custom;
#endif

/// <summary>
/// Executes actions when the associated <see cref="TabControl"/> changes selection.
/// </summary>
public class TabControlSelectionChangedTrigger : RoutedEventTriggerBase<SelectionChangedEventArgs>
{
    /// <inheritdoc />
    protected override RoutedEvent<SelectionChangedEventArgs> RoutedEvent
        => SelectingItemsControl.SelectionChangedEvent;

    static TabControlSelectionChangedTrigger()
    {
        EventRoutingStrategyProperty.OverrideMetadata<TabControlSelectionChangedTrigger>(
            new StyledPropertyMetadata<RoutingStrategies>(RoutingStrategies.Bubble));
    }
}
