// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using Avalonia.Animation;
using Avalonia.Controls;
using Avalonia.Xaml.Interactivity;

namespace Avalonia.Xaml.Interactions.Custom;

/// <summary>
/// Adds a <see cref="Transition"/> to the <see cref="StyledElement.Transitions"/> collection.
/// </summary>
public class AddTransitionAction : StyledElementAction
{
    /// <summary>
    /// Identifies the <seealso cref="Transition"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<TransitionBase?> TransitionProperty =
        AvaloniaProperty.Register<AddTransitionAction, TransitionBase?>(nameof(Transition));

    /// <summary>
    /// Identifies the <seealso cref="StyledElement"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<StyledElement?> StyledElementProperty =
        AvaloniaProperty.Register<AddTransitionAction, StyledElement?>(nameof(StyledElement));

    /// <summary>
    /// Gets or sets the transition to add. This is an avalonia property.
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
        if (target is null || Transition is null)
        {
            return false;
        }

        target.Transitions ??= [];
        target.Transitions.Add(Transition);

        return true;
    }
}
