using Avalonia.Controls;
using Avalonia.Xaml.Interactivity;

namespace Avalonia.Xaml.Interactions.Custom;

/// <summary>
/// 
/// </summary>
public class StartAnimationAction : AvaloniaObject, IAction
{
    /// <summary>
    /// 
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
    /// 
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="parameter"></param>
    /// <returns></returns>
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
