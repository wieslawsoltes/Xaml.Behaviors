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
public class ToolTipClosingTrigger : RoutedEventTrigger
{
    /// <inheritdoc />
    protected override RoutedEvent RoutedEvent
        => ToolTip.ToolTipClosingEvent;

    static ToolTipClosingTrigger()
    {
        EventRoutingStrategyProperty.OverrideMetadata<ToolTipClosingTrigger>(
            new StyledPropertyMetadata<RoutingStrategies>(
                defaultValue: RoutingStrategies.Direct));
    }
}
