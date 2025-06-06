// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using Avalonia.Input;
using Avalonia.Interactivity;

namespace Avalonia.Xaml.Interactions.Custom;

/// <summary>
/// 
/// </summary>
public class ScrollGestureEndedGestureTrigger : RoutedEventTriggerBase<ScrollGestureEndedEventArgs>
{
    /// <inheritdoc />
    protected override RoutedEvent<ScrollGestureEndedEventArgs> RoutedEvent
        => Gestures.ScrollGestureEndedEvent;

    static ScrollGestureEndedGestureTrigger()
    {
        EventRoutingStrategyProperty.OverrideMetadata<ScrollGestureEndedGestureTrigger>(
            new StyledPropertyMetadata<RoutingStrategies>(
                defaultValue: RoutingStrategies.Bubble));
    }
}
