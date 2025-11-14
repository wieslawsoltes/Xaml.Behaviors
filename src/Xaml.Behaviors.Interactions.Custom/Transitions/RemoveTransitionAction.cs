// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using Avalonia.Animation;
using Avalonia.Controls;
using Avalonia.Xaml.Interactivity;

namespace Avalonia.Xaml.Interactions.Custom;

/// <summary>
/// Removes a <see cref="TransitionBase"/> from the <see cref="Avalonia.Animation.Transitions"/> collection on the target element.
/// </summary>
public class RemoveTransitionAction : StyledElementAction
{
    /// <summary>
    /// Identifies the <seealso cref="TransitionBase"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<TransitionBase?> TransitionProperty =
        AvaloniaProperty.Register<RemoveTransitionAction, TransitionBase?>(nameof(Transition));

    /// <summary>
    /// Identifies the <seealso cref="StyledElement"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<StyledElement?> StyledElementProperty =
        AvaloniaProperty.Register<RemoveTransitionAction, StyledElement?>(nameof(StyledElement));

    /// <summary>
    /// Gets or sets the transition to remove. This is an avalonia property.
    /// </summary>
    public TransitionBase? Transition
    {
        get => GetValue(TransitionProperty);
        set => SetValue(TransitionProperty, value);
    }

    /// <summary>
    /// Gets or sets the target styled element. This is an avalonia property.
    /// </summary>
    [ResolveByName]
    public StyledElement? StyledElement
    {
        get => GetValue(StyledElementProperty);
        set => SetValue(StyledElementProperty, value);
    }

    /// <inheritdoc />
    public override object Execute(object? sender, object? parameter)
    {
        if (!IsEnabled)
        {
            return false;
        }

        var target = GetValue(StyledElementProperty) ?? sender as StyledElement;
        if (target?.Transitions is null)
        {
            return false;
        }

        if (Transition is not null)
        {
            return target.Transitions.Remove(Transition);
        }

        return false;
    }
}
