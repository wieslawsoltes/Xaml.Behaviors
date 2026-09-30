// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using System;
using Microsoft.UI.Xaml.Controls;

namespace Xaml.Interactions.Custom;

/// <content>
/// WinUI <see cref="ScrollViewer.ViewChanged"/> subscription of <see cref="ScrollChangedTrigger"/>.
/// </content>
public partial class ScrollChangedTrigger
{
    /// <inheritdoc />
    protected override IDisposable OnAttachedToVisualTreeOverride()
    {
        if (AssociatedObject is not ScrollViewer scrollViewer)
        {
            return DisposableAction.Empty;
        }

        scrollViewer.ViewChanged += OnViewChanged;
        return DisposableAction.Create(() => scrollViewer.ViewChanged -= OnViewChanged);
    }

    private void OnViewChanged(object? sender, ScrollViewerViewChangedEventArgs e)
    {
        if (!IsEnabled)
        {
            return;
        }

        Interaction.ExecuteActions(AssociatedObject, Actions, e);
    }
}
