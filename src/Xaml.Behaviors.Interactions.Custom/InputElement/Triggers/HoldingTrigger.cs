// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using Avalonia.Input;
using Avalonia.Interactivity;

namespace Avalonia.Xaml.Interactions.Custom;

/// <summary>
/// 
/// </summary>
public class HoldingTrigger : RoutedEventTriggerBase<HoldingRoutedEventArgs>
{
    /// <inheritdoc />
    protected override RoutedEvent<HoldingRoutedEventArgs> RoutedEvent 
        => InputElement.HoldingEvent;
    
    static HoldingTrigger()
    {
        EventRoutingStrategyProperty.OverrideMetadata<HoldingTrigger>(
            new StyledPropertyMetadata<RoutingStrategies>(
                defaultValue: RoutingStrategies.Bubble));
    }
}
