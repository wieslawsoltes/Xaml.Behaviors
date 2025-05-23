using System;
using Avalonia.Animation;
using Avalonia.Styling;

namespace Avalonia.Xaml.Interactions.Custom;

/// <summary>
/// Plays a simple fade in animation when the associated control is attached.
/// </summary>
public class FadeInBehavior : AttachedToVisualTreeBehavior<Visual>
{
    /// <summary>
    /// Gets or sets the delay before the animation starts.
    /// </summary>
    public static readonly StyledProperty<TimeSpan> InitialDelayProperty =
        AvaloniaProperty.Register<FadeInBehavior, TimeSpan>(nameof(InitialDelay), TimeSpan.FromMilliseconds(500));

    /// <summary>
    /// Gets or sets the duration of the fade in animation.
    /// </summary>
    public static readonly StyledProperty<TimeSpan> DurationProperty =
        AvaloniaProperty.Register<FadeInBehavior, TimeSpan>(nameof(Duration), TimeSpan.FromMilliseconds(250));

    /// <summary>
    /// 
    /// </summary>
    public TimeSpan InitialDelay
    {
        get => GetValue(InitialDelayProperty);
        set => SetValue(InitialDelayProperty, value);
    }

    /// <summary>
    /// 
    /// </summary>
    public TimeSpan Duration
    {
        get => GetValue(DurationProperty);
        set => SetValue(DurationProperty, value);
    }

    /// <summary>
    /// Called when the behavior is attached to the visual tree.
    /// </summary>
    protected override System.IDisposable OnAttachedToVisualTreeOverride()
    {
        if (AssociatedObject is null)
        {
            return DisposableAction.Empty;
        }

        var totalDuration = InitialDelay + Duration;

        var animation = new Animation.Animation
        {
            Duration = totalDuration,
            Children =
            {
                new KeyFrame {KeyTime = TimeSpan.Zero, Setters = {new Setter(Visual.OpacityProperty, 0d),}},
                new KeyFrame {KeyTime = InitialDelay, Setters = {new Setter(Visual.OpacityProperty, 0d),}},
                new KeyFrame {KeyTime = Duration, Setters = {new Setter(Visual.OpacityProperty, 1d),}}
            }
        };
        animation.RunAsync(AssociatedObject);

        return DisposableAction.Empty;
    }
}
