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
/// Trigger that listens for the <see cref="ToolTip.ToolTipClosingEvent"/>.
/// </summary>
/// <remarks>
/// WinUI has no tooltip closing event: on Uno Platform the trigger handles <c>ToolTip.Closed</c> of the tooltip
/// assigned with <c>ToolTipService.ToolTip</c> to the associated element.
/// </remarks>
#if UNO
public partial class ToolTipClosingTrigger : RoutedEventTriggerBase
#else
public partial class ToolTipClosingTrigger : RoutedEventTrigger
#endif
{
#if !UNO
    /// <inheritdoc />
    protected override RoutedEvent RoutedEvent
        => ToolTip.ToolTipClosingEvent;
#endif

    static ToolTipClosingTrigger()
    {
        EventRoutingStrategyProperty.OverrideMetadata<ToolTipClosingTrigger>(
            new StyledPropertyMetadata<RoutingStrategies>(
                defaultValue: RoutingStrategies.Direct));
    }
}
