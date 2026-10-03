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
public class DoubleTappedTrigger : RoutedEventTriggerBase<DoubleTappedRoutedEventArgs>
{
    /// <inheritdoc />
    protected override RoutedEvent<DoubleTappedRoutedEventArgs> RoutedEvent
#else
public class DoubleTappedTrigger : RoutedEventTriggerBase<TappedEventArgs>
{
    /// <inheritdoc />
    protected override RoutedEvent<TappedEventArgs> RoutedEvent
#endif
        => InputElement.DoubleTappedEvent;

    static DoubleTappedTrigger()
    {
        EventRoutingStrategyProperty.OverrideMetadata<DoubleTappedTrigger>(
            new StyledPropertyMetadata<RoutingStrategies>(
                defaultValue: RoutingStrategies.Bubble));
    }
}
