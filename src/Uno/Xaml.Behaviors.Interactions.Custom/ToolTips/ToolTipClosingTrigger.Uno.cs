// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using System;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace Xaml.Interactions.Custom;

/// <content>
/// WinUI <see cref="ToolTip.Closed"/> subscription of <see cref="ToolTipClosingTrigger"/>.
/// </content>
public partial class ToolTipClosingTrigger
{
    /// <inheritdoc />
    protected override IDisposable OnAttachedToVisualTreeOverride()
    {
        if (AssociatedObject is null || ToolTipExtensions.GetOrCreateToolTip(AssociatedObject) is not { } toolTip)
        {
            return DisposableAction.Empty;
        }

        toolTip.Closed += OnClosed;
        return DisposableAction.Create(() => toolTip.Closed -= OnClosed);
    }

    private void OnClosed(object sender, RoutedEventArgs e)
    {
        if (!IsEnabled)
        {
            return;
        }

        Interaction.ExecuteActions(AssociatedObject, Actions, e);
    }
}
