using Avalonia.Controls;
using Avalonia.Xaml.Interactivity;

namespace Avalonia.Xaml.Interactions.Custom;

/// <summary>
/// Starts an animation built in code on the associated control.
/// </summary>
public class StartBuiltAnimationAction : AvaloniaObject, IAction
{
    /// <summary>
    /// Identifies the <see cref="Animation"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<Animation.Animation?> AnimationProperty =
        AvaloniaProperty.Register<StartBuiltAnimationAction, Animation.Animation?>(nameof(Animation));

    /// <summary>
    /// Identifies the <see cref="AnimationBuilder"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<IAnimationBuilder?> AnimationBuilderProperty =
        AvaloniaProperty.Register<StartBuiltAnimationAction, IAnimationBuilder?>(nameof(AnimationBuilder));

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
    public object Execute(object? sender, object? parameter)
    {
        if (sender is not Control control)
        {
            return false;
        }

        var animation = Animation ?? AnimationBuilder?.Build(control);
        if (animation is null)
        {
            return false;
        }

        _ = animation.RunAsync(control);
        return true;
    }
}
