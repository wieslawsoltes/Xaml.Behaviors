// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using Avalonia.Input;
using Avalonia.Interactivity;

namespace Avalonia.Xaml.Interactions.Custom;

/// <summary>
/// 
/// </summary>
public class PointerTouchPadGestureRotateGestureTrigger : RoutedEventTriggerBase<PointerDeltaEventArgs>
{
    /// <inheritdoc />
    protected override RoutedEvent<PointerDeltaEventArgs> RoutedEvent
        => Gestures.PointerTouchPadGestureRotateEvent;

    static PointerTouchPadGestureRotateGestureTrigger()
    {
        EventRoutingStrategyProperty.OverrideMetadata<PointerTouchPadGestureRotateGestureTrigger>(
            new StyledPropertyMetadata<RoutingStrategies>(
                defaultValue: RoutingStrategies.Bubble));
    }
}
