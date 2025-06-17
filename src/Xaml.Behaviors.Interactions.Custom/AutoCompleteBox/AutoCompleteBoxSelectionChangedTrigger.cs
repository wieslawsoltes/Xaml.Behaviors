// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using Avalonia.Controls;
using Avalonia.Interactivity;

namespace Avalonia.Xaml.Interactions.Custom;

/// <summary>
/// Triggers actions when the <see cref="AutoCompleteBox"/> selection changes.
/// </summary>
public class AutoCompleteBoxSelectionChangedTrigger : RoutedEventTriggerBase<SelectionChangedEventArgs>
{
    /// <inheritdoc />
    protected override RoutedEvent<SelectionChangedEventArgs> RoutedEvent
        => AutoCompleteBox.SelectionChangedEvent;

    static AutoCompleteBoxSelectionChangedTrigger()
    {
        EventRoutingStrategyProperty.OverrideMetadata<AutoCompleteBoxSelectionChangedTrigger>(
            new StyledPropertyMetadata<RoutingStrategies>(RoutingStrategies.Bubble));
    }
}
