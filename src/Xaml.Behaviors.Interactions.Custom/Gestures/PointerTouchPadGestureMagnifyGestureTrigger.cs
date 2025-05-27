// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using Avalonia.Input;
using Avalonia.Interactivity;

namespace Avalonia.Xaml.Interactions.Custom;

/// <summary>
/// 
/// </summary>
public class PointerTouchPadGestureMagnifyGestureTrigger : RoutedEventTriggerBase<PointerDeltaEventArgs>
{
    /// <inheritdoc />
    protected override RoutedEvent<PointerDeltaEventArgs> RoutedEvent
        => Gestures.PointerTouchPadGestureMagnifyEvent;

    static PointerTouchPadGestureMagnifyGestureTrigger()
    {
        EventRoutingStrategyProperty.OverrideMetadata<PointerTouchPadGestureMagnifyGestureTrigger>(
            new StyledPropertyMetadata<RoutingStrategies>(
                defaultValue: RoutingStrategies.Bubble));
    }
}
