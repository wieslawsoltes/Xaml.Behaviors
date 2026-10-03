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
/// Trigger that listens for the <see cref="ToolTip.ToolTipOpeningEvent"/>.
/// </summary>
/// <remarks>
/// WinUI has no cancelable tooltip opening event: on Uno Platform the trigger handles <c>ToolTip.Opened</c> of the
/// tooltip assigned with <c>ToolTipService.ToolTip</c> to the associated element.
/// </remarks>
#if UNO
public partial class ToolTipOpeningTrigger : RoutedEventTriggerBase
#else
public partial class ToolTipOpeningTrigger : RoutedEventTriggerBase<CancelRoutedEventArgs>
#endif
{
#if !UNO
    /// <inheritdoc />
    protected override RoutedEvent<CancelRoutedEventArgs> RoutedEvent
        => ToolTip.ToolTipOpeningEvent;
#endif

    static ToolTipOpeningTrigger()
    {
        EventRoutingStrategyProperty.OverrideMetadata<ToolTipOpeningTrigger>(
            new StyledPropertyMetadata<RoutingStrategies>(
                defaultValue: RoutingStrategies.Direct));
    }
}
