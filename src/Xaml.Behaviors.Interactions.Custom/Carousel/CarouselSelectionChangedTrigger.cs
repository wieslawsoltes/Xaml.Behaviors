// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
#if UNO
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Carousel = Microsoft.UI.Xaml.Controls.FlipView;
#else
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Interactivity;
#endif

#if UNO
namespace Xaml.Interactions.Custom;
#else
namespace Avalonia.Xaml.Interactions.Custom;
#endif

/// <summary>
/// Executes actions when the associated <see cref="Carousel"/> changes selection.
/// </summary>
/// <remarks>
/// On Uno Platform the trigger handles <c>Selector.SelectionChanged</c> of the associated WinUI <c>FlipView</c>
/// (a CLR event: the routing strategy does not apply).
/// </remarks>
#if UNO
public partial class CarouselSelectionChangedTrigger : RoutedEventTriggerBase
#else
public partial class CarouselSelectionChangedTrigger : RoutedEventTriggerBase<SelectionChangedEventArgs>
#endif
{
#if !UNO
    /// <inheritdoc />
    protected override RoutedEvent<SelectionChangedEventArgs> RoutedEvent
        => SelectingItemsControl.SelectionChangedEvent;
#endif

    static CarouselSelectionChangedTrigger()
    {
        EventRoutingStrategyProperty.OverrideMetadata<CarouselSelectionChangedTrigger>(
            new StyledPropertyMetadata<RoutingStrategies>(RoutingStrategies.Bubble));
    }
}
