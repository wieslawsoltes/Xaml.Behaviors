// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using Avalonia.Input;
using Avalonia.Interactivity;

namespace Avalonia.Xaml.Interactions.Custom;

/// <summary>
/// 
/// </summary>
public class LostFocusTrigger : RoutedEventTriggerBase<FocusChangedEventArgs>
{
    /// <inheritdoc />
    protected override RoutedEvent<FocusChangedEventArgs> RoutedEvent 
        => InputElement.LostFocusEvent;

    static LostFocusTrigger()
    {
        EventRoutingStrategyProperty.OverrideMetadata<LostFocusTrigger>(
            new StyledPropertyMetadata<RoutingStrategies>(
                defaultValue: RoutingStrategies.Bubble));
    }
}
