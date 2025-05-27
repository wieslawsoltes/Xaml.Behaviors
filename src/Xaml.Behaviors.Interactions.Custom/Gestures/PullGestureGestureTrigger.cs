// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using Avalonia.Input;
using Avalonia.Interactivity;

namespace Avalonia.Xaml.Interactions.Custom;

/// <summary>
/// 
/// </summary>
public class PullGestureGestureTrigger : RoutedEventTriggerBase<PullGestureEventArgs>
{
    /// <inheritdoc />
    protected override RoutedEvent<PullGestureEventArgs> RoutedEvent
        => Gestures.PullGestureEvent;

    static PullGestureGestureTrigger()
    {
        EventRoutingStrategyProperty.OverrideMetadata<PullGestureGestureTrigger>(
            new StyledPropertyMetadata<RoutingStrategies>(
                defaultValue: RoutingStrategies.Bubble));
    }
}
