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
