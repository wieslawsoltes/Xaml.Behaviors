// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using Avalonia.Input;
using Avalonia.Interactivity;

namespace Avalonia.Xaml.Interactions.Custom;

/// <summary>
/// 
/// </summary>
public class PointerMovedTrigger : RoutedEventTriggerBase<PointerEventArgs>
{
    /// <inheritdoc />
    protected override RoutedEvent<PointerEventArgs> RoutedEvent 
        => InputElement.PointerMovedEvent;

    static PointerMovedTrigger()
    {
        EventRoutingStrategyProperty.OverrideMetadata<PointerMovedTrigger>(
            new StyledPropertyMetadata<RoutingStrategies>(
                defaultValue: RoutingStrategies.Tunnel | RoutingStrategies.Bubble));
    }
}
