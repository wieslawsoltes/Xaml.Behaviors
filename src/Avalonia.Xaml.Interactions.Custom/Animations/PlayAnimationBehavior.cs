using Avalonia.Animation;
using Avalonia.Styling;

namespace Avalonia.Xaml.Interactions.Custom;

/// <summary>
/// Plays a specified <see cref="Animation"/> when the associated element is attached to the visual tree.
/// </summary>
public class PlayAnimationBehavior : AttachedToVisualTreeBehavior<Visual>
{
    /// <summary>
    /// Identifies the <see cref="Animation"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<Animation?> AnimationProperty =
        AvaloniaProperty.Register<PlayAnimationBehavior, Animation?>(nameof(Animation));

    /// <summary>
    /// Gets or sets the animation that will be played. This is an avalonia property.
    /// </summary>
    [Content]
    public Animation? Animation
    {
        get => GetValue(AnimationProperty);
        set => SetValue(AnimationProperty, value);
    }

    /// <inheritdoc />
    protected override IDisposable OnAttachedToVisualTreeOverride()
    {
        if (AssociatedObject is null || Animation is null)
        {
            return DisposableAction.Empty;
        }

        _ = Animation.RunAsync(AssociatedObject);
        return DisposableAction.Empty;
    }
}
