// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using System.Linq;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Xaml.Interactivity;

namespace Avalonia.Xaml.Interactions.Custom;

/// <summary>
/// Trigger that executes actions when the associated <see cref="TabDragOutsideBehavior"/> reports that the tab was dragged outside.
/// </summary>
public class TabDragOutsideTrigger : StyledElementTrigger<TabItem>
{
    private TabDragOutsideBehavior? _behavior;

    /// <inheritdoc />
    protected override void OnAttachedToVisualTree()
    {
        if (AssociatedObject is not null)
        {
            _behavior = Interaction.GetBehaviors(AssociatedObject)
                .OfType<TabDragOutsideBehavior>()
                .FirstOrDefault();
            if (_behavior is not null)
            {
                _behavior.DragOutside += OnDragOutside;
            }
        }
    }

    /// <inheritdoc />
    protected override void OnDetachedFromVisualTree()
    {
        if (_behavior is not null)
        {
            _behavior.DragOutside -= OnDragOutside;
            _behavior = null;
        }
    }

    private void OnDragOutside(object? sender, PointerEventArgs e)
    {
        Execute(e);
    }

    private void Execute(object? parameter)
    {
        if (!IsEnabled || AssociatedObject is null)
        {
            return;
        }

        Interaction.ExecuteActions(AssociatedObject, Actions, parameter);
    }
}
