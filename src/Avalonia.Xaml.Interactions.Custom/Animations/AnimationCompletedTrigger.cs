using System.Threading.Tasks;
using Avalonia.Threading;
using Avalonia.Xaml.Interactivity;

namespace Avalonia.Xaml.Interactions.Custom;

/// <summary>
/// Runs a specified <see cref="Animation.Animation"/> and executes actions when it completes.
/// </summary>
public class AnimationCompletedTrigger : AttachedToVisualTreeTrigger
{
    /// <summary>
    /// Identifies the <see cref="Animation"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<Animation.Animation?> AnimationProperty =
        AvaloniaProperty.Register<AnimationCompletedTrigger, Animation.Animation?>(nameof(Animation));

    /// <summary>
    /// Gets or sets the animation to run. This is an avalonia property.
    /// </summary>
    public Animation.Animation? Animation
    {
        get => GetValue(AnimationProperty);
        set => SetValue(AnimationProperty, value);
    }

    /// <inheritdoc />
    protected override void OnAttachedToVisualTree()
    {
        if (AssociatedObject is null)
        {
            return;
        }

        if (Animation is null)
        {
            Execute(parameter: null);
            return;
        }

        _ = Run();

        async Task Run()
        {
            await Animation.RunAsync(AssociatedObject);
            Dispatcher.UIThread.Post(() => Execute(parameter: null));
        }
    }

    private void Execute(object? parameter)
    {
        if (!IsEnabled)
        {
            return;
        }

        Interaction.ExecuteActions(AssociatedObject, Actions, parameter);
    }
}
