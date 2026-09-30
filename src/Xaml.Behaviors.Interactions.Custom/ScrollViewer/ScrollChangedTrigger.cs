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
/// <remarks>
/// On Uno Platform the trigger handles <c>ScrollViewer.ViewChanged</c> of the associated WinUI <c>ScrollViewer</c>
/// (a CLR event: the routing strategy does not apply).
/// </remarks>
#if UNO
public partial class ScrollChangedTrigger : RoutedEventTriggerBase
#else
public partial class ScrollChangedTrigger : RoutedEventTriggerBase<ScrollChangedEventArgs>
#endif
{
#if !UNO
    /// <inheritdoc />
    protected override RoutedEvent<ScrollChangedEventArgs> RoutedEvent
        => ScrollViewer.ScrollChangedEvent;
#endif

    static ScrollChangedTrigger()
    {
        EventRoutingStrategyProperty.OverrideMetadata<ScrollChangedTrigger>(
            new StyledPropertyMetadata<RoutingStrategies>(RoutingStrategies.Bubble));
    }
}
