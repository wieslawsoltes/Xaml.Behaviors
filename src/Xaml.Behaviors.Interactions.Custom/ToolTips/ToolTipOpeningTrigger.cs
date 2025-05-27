// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using Avalonia.Controls;
using Avalonia.Interactivity;

namespace Avalonia.Xaml.Interactions.Custom;

/// <summary>
/// Trigger that listens for the <see cref="ToolTip.ToolTipOpeningEvent"/>.
/// </summary>
public class ToolTipOpeningTrigger : RoutedEventTriggerBase<CancelRoutedEventArgs>
{
    /// <inheritdoc />
    protected override RoutedEvent<CancelRoutedEventArgs> RoutedEvent
        => ToolTip.ToolTipOpeningEvent;

    static ToolTipOpeningTrigger()
    {
        EventRoutingStrategyProperty.OverrideMetadata<ToolTipOpeningTrigger>(
            new StyledPropertyMetadata<RoutingStrategies>(
                defaultValue: RoutingStrategies.Bubble));
    }
}
