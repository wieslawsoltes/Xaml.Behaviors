using Avalonia.Controls;
using Avalonia.Xaml.Interactivity;

namespace Avalonia.Xaml.Interactions.Custom;

/// <summary>
/// Starts an <see cref="Animation.Animation"/> on the associated control.
/// </summary>
public class StartAnimationAction : AvaloniaObject, IAction
{
    /// <summary>
    /// Gets or sets the animation to run.
    /// </summary>
    public static readonly StyledProperty<Animation.Animation?> AnimationProperty =
        AvaloniaProperty.Register<StartAnimationAction, Animation.Animation?>(nameof(Animation));

    /// <summary>
    /// 
    /// </summary>
    public Animation.Animation? Animation
    {
        get => GetValue(AnimationProperty);
        set => SetValue(AnimationProperty, value);
    }

    /// <summary>
    /// Executes the action.
    /// </summary>
    /// <param name="sender">The control initiating the action.</param>
    /// <param name="parameter">Optional parameter.</param>
    /// <returns>True if the animation was started; otherwise, false.</returns>
    public object Execute(object? sender, object? parameter)
    {
        if (sender is not Control control || Animation is null)
        {
            return false;
        }

        _ = Animation.RunAsync(control);

        return true;
    }
}
