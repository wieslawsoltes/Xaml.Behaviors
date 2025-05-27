// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using System;
using Avalonia.Controls;

namespace Avalonia.Xaml.Interactions.Custom;

/// <summary>
/// Runs an animation when the associated control is attached to the visual tree.
/// </summary>
public class AnimateOnAttachedBehavior : AttachedToVisualTreeBehavior<Control>
{
    /// <summary>
    /// Identifies the <see cref="Animation"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<Animation.Animation?> AnimationProperty =
        AvaloniaProperty.Register<AnimateOnAttachedBehavior, Animation.Animation?>(nameof(Animation));

    /// <summary>
    /// Identifies the <see cref="AnimationBuilder"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<IAnimationBuilder?> AnimationBuilderProperty =
        AvaloniaProperty.Register<AnimateOnAttachedBehavior, IAnimationBuilder?>(nameof(AnimationBuilder));

    /// <summary>
    /// Gets or sets the animation to run.
    /// </summary>
    public Animation.Animation? Animation
    {
        get => GetValue(AnimationProperty);
        set => SetValue(AnimationProperty, value);
    }

    /// <summary>
    /// Gets or sets the animation builder used to create an animation.
    /// </summary>
    public IAnimationBuilder? AnimationBuilder
    {
        get => GetValue(AnimationBuilderProperty);
        set => SetValue(AnimationBuilderProperty, value);
    }

    /// <inheritdoc />
    protected override IDisposable OnAttachedToVisualTreeOverride()
    {
        if (AssociatedObject is null)
        {
            return DisposableAction.Empty;
        }

        var animation = Animation ?? AnimationBuilder?.Build(AssociatedObject);
        if (animation is not null)
        {
            _ = animation.RunAsync(AssociatedObject);
        }

        return DisposableAction.Empty;
    }
}
