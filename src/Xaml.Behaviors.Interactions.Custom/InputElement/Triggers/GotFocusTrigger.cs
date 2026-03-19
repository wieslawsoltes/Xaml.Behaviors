// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using Avalonia.Input;
using Avalonia.Interactivity;

namespace Avalonia.Xaml.Interactions.Custom;

/// <summary>
/// 
/// </summary>
public class GotFocusTrigger : RoutedEventTriggerBase<FocusChangedEventArgs>
{
    /// <inheritdoc />
    protected override RoutedEvent<FocusChangedEventArgs> RoutedEvent 
        => InputElement.GotFocusEvent;

    static GotFocusTrigger()
    {
        EventRoutingStrategyProperty.OverrideMetadata<GotFocusTrigger>(
            new StyledPropertyMetadata<RoutingStrategies>(
                defaultValue: RoutingStrategies.Bubble));
    }
}
