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
#if UNO
public class RightTappedGestureTrigger : RoutedEventTriggerBase<RightTappedRoutedEventArgs>
{
    /// <inheritdoc />
    protected override RoutedEvent<RightTappedRoutedEventArgs> RoutedEvent
#else
public class RightTappedGestureTrigger : RoutedEventTriggerBase<TappedEventArgs>
{
    /// <inheritdoc />
    protected override RoutedEvent<TappedEventArgs> RoutedEvent
#endif
        => InputElement.RightTappedEvent;

    static RightTappedGestureTrigger()
    {
        EventRoutingStrategyProperty.OverrideMetadata<RightTappedGestureTrigger>(
            new StyledPropertyMetadata<RoutingStrategies>(
                defaultValue: RoutingStrategies.Bubble));
    }
}
