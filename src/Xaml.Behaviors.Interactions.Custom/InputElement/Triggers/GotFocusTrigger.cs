// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using Avalonia.Input;
using Avalonia.Interactivity;

namespace Avalonia.Xaml.Interactions.Custom;

/// <summary>
/// 
/// </summary>
public class GotFocusTrigger : RoutedEventTriggerBase<GotFocusEventArgs>
{
    /// <inheritdoc />
    protected override RoutedEvent<GotFocusEventArgs> RoutedEvent 
        => InputElement.GotFocusEvent;

    static GotFocusTrigger()
    {
        EventRoutingStrategyProperty.OverrideMetadata<GotFocusTrigger>(
            new StyledPropertyMetadata<RoutingStrategies>(
                defaultValue: RoutingStrategies.Bubble));
    }
}
