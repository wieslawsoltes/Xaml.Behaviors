// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using System;
using Avalonia.Controls.Primitives;
using Avalonia.Xaml.Interactivity;

namespace Avalonia.Xaml.Interactions.Custom;

/// <summary>
/// Executes actions when the <see cref="Popup.Closed"/> event is raised.
/// </summary>
public class PopupClosedTrigger : StyledElementTrigger<Popup>
{
    /// <inheritdoc />
    protected override void OnAttachedToVisualTree()
    {
        if (AssociatedObject is not null)
        {
            AssociatedObject.Closed += OnClosed;
        }
    }

    /// <inheritdoc />
    protected override void OnDetachedFromVisualTree()
    {
        if (AssociatedObject is not null)
        {
            AssociatedObject.Closed -= OnClosed;
        }
    }

    private void OnClosed(object? sender, EventArgs e)
    {
        Execute(e);
    }

    private void Execute(object? parameter)
    {
        if (!IsEnabled)
        {
            return;
        }

        if (AssociatedObject is not null)
        {
            Interaction.ExecuteActions(AssociatedObject, Actions, parameter);
        }
    }
}
