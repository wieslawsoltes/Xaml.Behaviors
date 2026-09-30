// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
#if UNO
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Input;
#else
using Avalonia.Input;
using Avalonia.Interactivity;
#endif

#if UNO
namespace Xaml.Interactions.Custom;
#else
namespace Avalonia.Xaml.Interactions.Custom;
#endif

/// <summary>
/// 
/// </summary>
public class PinchEndedGestureTrigger : RoutedEventTriggerBase<PinchEndedEventArgs>
{
    /// <inheritdoc />
    protected override RoutedEvent<PinchEndedEventArgs> RoutedEvent
        => InputElement.PinchEndedEvent;

    static PinchEndedGestureTrigger()
    {
        EventRoutingStrategyProperty.OverrideMetadata<PinchEndedGestureTrigger>(
            new StyledPropertyMetadata<RoutingStrategies>(
                defaultValue: RoutingStrategies.Bubble));
    }
}
