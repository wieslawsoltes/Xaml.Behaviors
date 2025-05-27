// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using Avalonia.Animation;
using Avalonia.Controls;

namespace Avalonia.Xaml.Interactions.Custom;

/// <summary>
/// Sets the <see cref="StyledElement.Transitions"/> collection on the associated control when attached.
/// </summary>
public class TransitionsBehavior : AttachedToVisualTreeBehavior<Control>
{
    /// <summary>
    /// Identifies the <seealso cref="TransitionsSource"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<Transitions?> TransitionsSourceProperty =
        AvaloniaProperty.Register<TransitionsBehavior, Transitions?>(nameof(TransitionsSource));

    private Transitions? _oldTransitions;

    /// <summary>
    /// Gets or sets the transitions collection to apply. This is an avalonia property.
    /// </summary>
    public Transitions? TransitionsSource
    {
        get => GetValue(TransitionsProperty);
        set => SetValue(TransitionsProperty, value);
    }

    /// <inheritdoc />
    protected override System.IDisposable OnAttachedToVisualTreeOverride()
    {
        if (AssociatedObject is null)
        {
            return DisposableAction.Empty;
        }

        _oldTransitions = AssociatedObject.Transitions;
        AssociatedObject.Transitions = TransitionsSource;

        return DisposableAction.Create(() =>
        {
            if (AssociatedObject is not null)
            {
                AssociatedObject.Transitions = _oldTransitions;
            }
        });
    }
}
