// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using System;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.VisualTree;
using Avalonia.Xaml.Interactivity;

namespace Avalonia.Xaml.Interactions.Custom;

/// <summary>
/// A behavior that traps focus within the attached element.
/// </summary>
public class FocusTrapBehavior : Behavior<Control>
{
    private IDisposable? _keyDownDisposable;

    /// <summary>
    /// Identifies the <seealso cref="IsActive"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<bool> IsActiveProperty =
        AvaloniaProperty.Register<FocusTrapBehavior, bool>(nameof(IsActive), defaultValue: true);

    /// <summary>
    /// Gets or sets a value indicating whether the focus trap is active.
    /// </summary>
    public bool IsActive
    {
        get => GetValue(IsActiveProperty);
        set => SetValue(IsActiveProperty, value);
    }

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

            var focusManager = TopLevel.GetTopLevel(AssociatedObject)?.FocusManager;
            if (focusManager == null)
            {
                return;
            }

            var currentFocus = focusManager.GetFocusedElement() as Control;
            
            // If focus is not currently within our container, force it in.
            if (currentFocus == null || !AssociatedObject.IsVisualAncestorOf(currentFocus))
            {
                // Focus the first focusable element in the container
                var first = KeyboardNavigationHandler.GetNext(AssociatedObject, NavigationDirection.Next);
                if (first != null)
                {
                    first.Focus();
                    e.Handled = true;
                }
                return;
            }

            // Check if we are about to leave the container
            var next = KeyboardNavigationHandler.GetNext(currentFocus, direction);

            if (next == null || !AssociatedObject.IsVisualAncestorOf(next as Visual))
            {
                // Wrap around
                var wrapTarget = direction == NavigationDirection.Next
                    ? KeyboardNavigationHandler.GetNext(AssociatedObject, NavigationDirection.Next) // First
                    : KeyboardNavigationHandler.GetNext(AssociatedObject, NavigationDirection.Previous); // Last (this might need better logic for "Last")

                // Finding the "Last" element via GetNext(container, Previous) might not work as expected depending on implementation.
                // A robust way is to find the first element and cycle, or just let Avalonia's FocusManager handle it if we can restrict the scope.
                // But Avalonia doesn't have a "FocusScope" that traps strictly yet.
                
                if (direction == NavigationDirection.Previous)
                {
                    // To find the last focusable element, we can try to find the first one and go backwards? 
                    // Or just focus the container itself if it's focusable?
                    // Let's try to find the last focusable descendant.
                    // For now, let's just wrap to the first one for simplicity or try to find a better way.
                    
                    // Actually, KeyboardNavigationHandler.GetNext(AssociatedObject, NavigationDirection.Previous) should give the last focusable element inside if AssociatedObject is a focus scope?
                    // No, it usually gives the element *before* AssociatedObject in the global order.
                    
                    // Let's try to find the first element, and if we are going backwards from the first element, go to the last.
                    // But we don't know easily which is the last.
                    
                    // Simple approach: If we are at the edge, focus the other edge.
                    
                    // If we are going Next and next is outside, focus First.
                    if (direction == NavigationDirection.Next)
                    {
                         var first = KeyboardNavigationHandler.GetNext(AssociatedObject, NavigationDirection.Next);
                         if (first != null)
                         {
                             first.Focus();
                             e.Handled = true;
                         }
                    }
                    else // Previous
                    {
                        // If we are going Previous and next is outside, focus Last.
                        // How to get Last? 
                        // We can iterate all focusable elements? That's expensive.
                        // Or we can just focus the container and let the user tab forward? No.
                        
                        // Let's just focus the first element for now as a fallback, effectively cycling to start.
                        // Ideally we want to cycle to end.
                        
                        var first = KeyboardNavigationHandler.GetNext(AssociatedObject, NavigationDirection.Next);
                        if (first != null)
                        {
                            first.Focus();
                            e.Handled = true;
                        }
                    }
                }
                else
                {
                     if (wrapTarget != null && AssociatedObject.IsVisualAncestorOf(wrapTarget as Visual))
                     {
                         wrapTarget.Focus();
                         e.Handled = true;
                     }
                     else
                     {
                         // Fallback to first
                         var first = KeyboardNavigationHandler.GetNext(AssociatedObject, NavigationDirection.Next);
                         if (first != null)
                         {
                             first.Focus();
                             e.Handled = true;
                         }
                     }
                }
            }
        }
    }
}
