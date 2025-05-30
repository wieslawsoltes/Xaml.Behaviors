// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Interactivity;

namespace Avalonia.Xaml.Interactions.Custom;

/// <summary>
/// Executes actions when the associated <see cref="ComboBox"/> changes selection.
/// </summary>
public class ComboBoxSelectionChangedTrigger : RoutedEventTriggerBase<SelectionChangedEventArgs>
{
    /// <inheritdoc />
    protected override RoutedEvent<SelectionChangedEventArgs> RoutedEvent
        => SelectingItemsControl.SelectionChangedEvent;

    static ComboBoxSelectionChangedTrigger()
    {
        EventRoutingStrategyProperty.OverrideMetadata<ComboBoxSelectionChangedTrigger>(
            new StyledPropertyMetadata<RoutingStrategies>(RoutingStrategies.Bubble));
    }
}
