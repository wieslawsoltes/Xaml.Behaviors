// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
#if UNO
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
#else
using Avalonia.Controls;
using Avalonia.Interactivity;
#endif

#if UNO
namespace Xaml.Interactions.Custom;
#else
namespace Avalonia.Xaml.Interactions.Custom;
#endif

/// <summary>
/// Executes actions when the <see cref="ScrollViewer.ScrollChanged"/> event occurs.
/// </summary>
public class ScrollChangedTrigger : RoutedEventTriggerBase<ScrollChangedEventArgs>
{
    /// <inheritdoc />
    protected override RoutedEvent<ScrollChangedEventArgs> RoutedEvent
        => ScrollViewer.ScrollChangedEvent;

    static ScrollChangedTrigger()
    {
        EventRoutingStrategyProperty.OverrideMetadata<ScrollChangedTrigger>(
            new StyledPropertyMetadata<RoutingStrategies>(RoutingStrategies.Bubble));
    }
}
