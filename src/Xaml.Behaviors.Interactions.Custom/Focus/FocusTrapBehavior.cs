// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using System;
#if UNO
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Xaml.Interactivity;
#else
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.VisualTree;
using Avalonia.Xaml.Interactivity;
#endif

#if UNO
namespace Xaml.Interactions.Custom;
#else
namespace Avalonia.Xaml.Interactions.Custom;
#endif

/// <summary>
/// A behavior that traps focus within the attached element.
/// </summary>
public partial class FocusTrapBehavior : Behavior<Control>
{

    /// <summary>
    /// Gets or sets a value indicating whether the focus trap is active.
    /// </summary>
    [StyledProperty(DefaultValue = true)]
    public partial bool IsActive { get; set; }

    /// <inheritdoc />
    protected override void OnAttached()
    {
        base.OnAttached();
        if (AssociatedObject != null)
        {
            AssociatedObject.KeyDown += OnKeyDown;
        }
    }

    /// <inheritdoc />
    protected override void OnDetaching()
    {
        if (AssociatedObject != null)
        {
            AssociatedObject.KeyDown -= OnKeyDown;
        }
        base.OnDetaching();
    }

    private void OnKeyDown(object? sender, KeyEventArgs e)
    {
        if (!IsActive || AssociatedObject == null)
        {
            return;
        }

        if (e.Key == Key.Tab)
        {
            var direction = e.KeyModifiers.HasFlag(KeyModifiers.Shift)
                ? NavigationDirection.Previous
                : NavigationDirection.Next;

            var topLevel = TopLevel.GetTopLevel(AssociatedObject);
            var focusManager = topLevel?.FocusManager;
            if (focusManager == null)
            {
                return;
            }

            var currentFocus = focusManager.GetFocusedElement() as Control;

            if (currentFocus == null || !AssociatedObject.IsVisualAncestorOf(currentFocus))
            {
                var boundary = GetBoundaryElement(direction);
                if (boundary != null)
                {
                    boundary.Focus();
                    e.Handled = true;
                }
                return;
            }

            var next = topLevel is null
                ? null
                : FocusNavigationHelper.FindAdjacent(topLevel, currentFocus, direction, wrap: false);

            if (next == null || next is not Visual nextVisual || !AssociatedObject.IsVisualAncestorOf(nextVisual))
            {
                var wrapTarget = GetBoundaryElement(direction);
                if (wrapTarget is not null)
                {
                    wrapTarget.Focus();
                    e.Handled = true;
                }
            }
        }
    }

    private Control? GetBoundaryElement(NavigationDirection direction)
    {
        return AssociatedObject is null
            ? null
            : FocusNavigationHelper.FindBoundary(AssociatedObject, direction) as Control;
    }
}
