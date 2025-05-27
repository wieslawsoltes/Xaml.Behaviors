// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using Avalonia.Controls;
using Avalonia.Metadata;
using Avalonia.Xaml.Interactivity;

namespace Avalonia.Xaml.Interactions.Custom;

/// <summary>
/// Starts an <see cref="Animation.Animation"/> on a specified control when executed.
/// </summary>
public class BeginAnimationAction : StyledElementAction
{
    /// <summary>
    /// Identifies the <see cref="Animation"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<Animation.Animation?> AnimationProperty =
        AvaloniaProperty.Register<BeginAnimationAction, Animation.Animation?>(nameof(Animation));

    /// <summary>
    /// Identifies the <see cref="TargetControl"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<Control?> TargetControlProperty =
        AvaloniaProperty.Register<BeginAnimationAction, Control?>(nameof(TargetControl));

    /// <summary>
    /// Gets or sets the animation to run. This is an avalonia property.
    /// </summary>
    [Content]
    public Animation.Animation? Animation
    {
        get => GetValue(AnimationProperty);
        set => SetValue(AnimationProperty, value);
    }

    /// <summary>
    /// Gets or sets the control on which the animation will run. This is an avalonia property.
    /// </summary>
    [ResolveByName]
    public Control? TargetControl
    {
        get => GetValue(TargetControlProperty);
        set => SetValue(TargetControlProperty, value);
    }

    /// <inheritdoc />
    public override object Execute(object? sender, object? parameter)
    {
        if (!IsEnabled)
        {
            return false;
        }

        var control = TargetControl ?? sender as Control;
        if (control is null || Animation is null)
        {
            return false;
        }

        _ = Animation.RunAsync(control);
        return true;
    }
}
