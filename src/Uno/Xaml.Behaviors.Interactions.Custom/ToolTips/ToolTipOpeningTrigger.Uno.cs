// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using System;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace Xaml.Interactions.Custom;

/// <content>
/// WinUI <see cref="ToolTip.Opened"/> subscription of <see cref="ToolTipOpeningTrigger"/>.
/// </content>
public partial class ToolTipOpeningTrigger
{
    /// <inheritdoc />
    protected override IDisposable OnAttachedToVisualTreeOverride()
    {
        if (AssociatedObject is null || ToolTipExtensions.GetOrCreateToolTip(AssociatedObject) is not { } toolTip)
        {
            return DisposableAction.Empty;
        }

        toolTip.Opened += OnOpened;
        return DisposableAction.Create(() => toolTip.Opened -= OnOpened);
    }

    private void OnOpened(object sender, RoutedEventArgs e)
    {
        if (!IsEnabled)
        {
            return;
        }

        Interaction.ExecuteActions(AssociatedObject, Actions, e);
    }
}
